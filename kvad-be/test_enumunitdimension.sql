-- Test Script for EnumUnitDimension Requirements
-- Run this in PostgreSQL after applying the migration

BEGIN;

-- Test 1: Verify current data structure
SELECT 'Test 1: Current EnumUnitDimensions' as test_name;
SELECT * FROM "EnumUnitDimensions" ORDER BY "Id";

-- Test 2: Verify Unit.Dimension arrays have correct length (should be 7)
SELECT 'Test 2: Unit Dimension Array Lengths' as test_name;
SELECT "Symbol", array_length("Dimension", 1) as dimension_length 
FROM "Units" 
LIMIT 5;

-- Test 3: Try to insert a non-sequential ID (should fail)
SELECT 'Test 3: Testing sequential ID constraint (should fail)' as test_name;
DO $$
BEGIN
    INSERT INTO "EnumUnitDimensions" ("Id", "Symbol", "Name") 
    VALUES (10, 'TEST', 'Test Dimension');
    RAISE NOTICE 'ERROR: Sequential constraint failed to prevent insertion!';
EXCEPTION
    WHEN check_violation THEN
        RAISE NOTICE 'SUCCESS: Sequential ID constraint working correctly';
END $$;

-- Test 4: Try to delete a dimension that's in use (should fail)
SELECT 'Test 4: Testing deletion prevention (should fail)' as test_name;
DO $$
BEGIN
    DELETE FROM "EnumUnitDimensions" WHERE "Id" = 0; -- Length dimension
    RAISE NOTICE 'ERROR: Deletion constraint failed to prevent removal!';
EXCEPTION
    WHEN OTHERS THEN
        RAISE NOTICE 'SUCCESS: Deletion prevention working correctly';
END $$;

-- Test 5: Insert a new sequential dimension (should succeed and expand arrays)
SELECT 'Test 5: Testing array expansion on new dimension' as test_name;
INSERT INTO "EnumUnitDimensions" ("Id", "Symbol", "Name") 
VALUES (7, 'X', 'Extra Dimension');

-- Verify arrays were expanded
SELECT "Symbol", array_length("Dimension", 1) as new_dimension_length 
FROM "Units" 
LIMIT 3;

-- Test 6: Try to insert a Unit with wrong array length (should fail)
SELECT 'Test 6: Testing Unit array length validation (should fail)' as test_name;
DO $$
BEGIN
    INSERT INTO "Units" ("Symbol", "Name", "Quantity", "Dimension", "Prefixable", "Factor", "UnitKind")
    VALUES ('TEST', 'Test Unit', 'Test', ARRAY[1,0,0,0,0]::smallint[], true, '{"Numerator":1,"Denominator":1}', 'linear');
    RAISE NOTICE 'ERROR: Array length validation failed!';
EXCEPTION
    WHEN OTHERS THEN
        RAISE NOTICE 'SUCCESS: Unit array length validation working correctly';
END $$;

-- Test 7: Insert a Unit with correct array length (should succeed)
SELECT 'Test 7: Testing Unit with correct array length (should succeed)' as test_name;
INSERT INTO "Units" ("Symbol", "Name", "Quantity", "Dimension", "Prefixable", "Factor", "UnitKind")
VALUES ('TEST', 'Test Unit', 'Test', ARRAY[1,0,0,0,0,0,0,0]::smallint[], true, '{"Numerator":1,"Denominator":1}', 'linear');

-- Cleanup
DELETE FROM "Units" WHERE "Symbol" = 'TEST';
DELETE FROM "EnumUnitDimensions" WHERE "Id" = 7;

ROLLBACK;

SELECT 'All tests completed!' as status;