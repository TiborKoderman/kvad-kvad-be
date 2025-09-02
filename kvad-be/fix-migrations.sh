#!/bin/bash

# Fix empty using statements in EF Core generated files
# This happens because some model classes don't have explicit namespaces

echo "Fixing empty using statements in migration files..."

# Fix all migration files that have empty using statements
find ./Migrations -name "*.cs" -type f -exec sed -i '/^using ;$/d' {} \;

echo "Done! Empty using statements have been removed from migration files."
