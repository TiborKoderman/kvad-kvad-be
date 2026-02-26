// Rational32 is a type alias for Rational<int> – a rational number backed
// by 32-bit signed integers.  Use the adaptive Rational wrapper if you need
// automatic promotion to larger storage on overflow.
global using Rational32 = Rational<int>;
