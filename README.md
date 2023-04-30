# GenericArithmetic

This generic implementation has been tested and supports performing arithmetic on numeric types such as BigInteger, Complex, Decimal, Double, Float, BigDecimal, BigRational, BigComplex, Fraction, Int16, Int32, Int64, UInt16, UInt32, UInt64, Short, Long, Byte, or any class that has arithmetic operator overloads.


#


### Generic Arithmetic Type Polynomial

* All classes and methods support a generic type T, which can be any data type that has arithmetic operator overloads. Explicitly, the following types have tests exercising all the standard arithmetic operations (Addition, Subtraction, Multiplication, Division, Exponentiation, Modulus, Square Root, Equality and Comparison (where applicable--Complex numbers are not orderable)) and so are well supported and come with verifiable proof that they work:
   * Every .NET CLR value type (Byte, Short, Long, Int16, Int32, Int64, UInt16, UInt32, UInt64, Float, Double, Decimal)
   * System.Numerics.BigInteger
   * System.Numerics.Complex
   * [ExtendedNumerics.BigComplex](https://github.com/AdamWhiteHat/BigComplex) (My arbitrary-precision complex number type library)
   * [ExtendedNumerics.BigDecimal](https://github.com/AdamWhiteHat/BigDecimal) (My arbitrary-precision floating-point number type library.)
   * [ExtendedNumerics.BigRational](https://github.com/AdamWhiteHat/BigRational)(My arbitrary precision rational number type library.)
   * [ExtendedNumerics.Fraction](https://github.com/AdamWhiteHat/BigRational/blob/master/BigRational/Fraction.cs) (My arbitrary-precision rational number type library.)
   * Any type that has the Addition, Multiplication and Exponentiation operator overloads (at a minimum).


#


# Other polynomial projects & numeric types

I've written a number of other polynomial implementations and numeric types catering to various specific scenarios. Depending on what you're trying to do, another implementation of this same library might be more appropriate. All of my polynomial projects should have feature parity, where appropriate[^1].

[^1]: For example, the ComplexPolynomial implementation may be missing certain operations (namely: Irreducibility), because such a notion does not make sense or is ill defined in the context of complex numbers).

* [GenericArithmetic](https://github.com/AdamWhiteHat/GenericArithmetic) - A core math library. Its a class of static methods that allows you to perform arithmetic on an arbitrary numeric type, represented by the generic type T, who's concrete type is decided by the caller. This is implemented using System.Linq.Expressions and reflection to resolve the type's static overloadable operator methods at runtime, so it works on all the .NET numeric types automagically, as well as any custom numeric type, provided it overloads the numeric operators and standard method names for other common functions (Min, Max, Abs, Sqrt, Parse, Sign, Log,  Round, etc.). Every generic arithmetic class listed below takes a dependency on this class.

* [Polynomial](https://github.com/AdamWhiteHat/Polynomial) - The original. A univariate polynomial that uses System.Numerics.BigInteger as the indeterminate type.
* [GenericPolynomial](https://github.com/AdamWhiteHat/GenericPolynomial) -  A univariate polynomial library that allows the indeterminate to be of an arbitrary type, as long as said type implements operator overloading. This is implemented dynamically, at run time, calling the operator overload methods using Linq.Expressions and reflection.
* [CSharp11Preview.GenericMath.Polynomial](https://github.com/AdamWhiteHat/CSharp11Preview.GenericMath.Polynomial) -  A univariate polynomial library that allows the indeterminate to be of an arbitrary type, but this version is implemented using C# 11's new Generic Math via static virtual members in interfaces.
>
* [MultivariatePolynomial](https://github.com/AdamWhiteHat/MultivariatePolynomial) - A multivariate polynomial (meaning more than one indeterminate, e.g. 2*X*Y^2) which uses BigInteger as the type for the indeterminates.
* [GenericMultivariatePolynomial](https://github.com/AdamWhiteHat/GenericMultivariatePolynomial) - A multivariate polynomial that allows the indeterminates to be of [the same] arbitrary type. GenericMultivariatePolynomial is to MultivariatePolynomial what GenericPolynomial is to Polynomial, and indeed is implemented using the same strategy as GenericPolynomial (i.e. dynamic calling of the operator overload methods at runtime using Linq.Expressions and reflection).
>
* [ComplexPolynomial](https://github.com/AdamWhiteHat/ComplexPolynomial) - A univariate polynomial library that has System.Numerics.Complex type indeterminates.
* [ComplexMultivariatePolynomial](https://github.com/AdamWhiteHat/ComplexMultivariatePolynomial) -  A multivariate polynomial library that has System.Numerics.Complex indeterminates.
>
* [BigDecimal](https://github.com/AdamWhiteHat/BigDecimal) - An arbitrary precision, base-10 floating point number class.
* [BigRational](https://github.com/AdamWhiteHat/BigRational) - Encodes a numeric value as an Integer + Fraction
* [BigComplex](https://github.com/AdamWhiteHat/BigComplex) - Essentially the same thing as System.Numerics.Complex but uses a System.Numerics.BigInteger type for the real and imaginary parts instead of a double.
>
* [IntervalArithmetic](https://github.com/AdamWhiteHat/IntervalArithmetic). Instead of representing a value as a single number, interval arithmetic represents each value as a mathematical interval, or range of possibilities, [a,b], and allows the standard arithmetic operations to be performed upon them too, adjusting or scaling the underlying interval range as appropriate. See [Wikipedia's article on Interval Arithmetic](https://en.wikipedia.org/wiki/Interval_arithmetic) for further information.
* [GNFS](https://github.com/AdamWhiteHat/GNFS) - A C# reference implementation of the General Number Field Sieve algorithm for the purpose of better understanding the General Number Field Sieve algorithm.
