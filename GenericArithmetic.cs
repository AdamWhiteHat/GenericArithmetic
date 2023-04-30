using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Globalization;

namespace ExtendedArithmetic
{
	public static class GenericArithmetic<T>
	{
		/// <summary>Gets a value that represents the number negative one (-1).</summary>
		public static T MinusOne;
		/// <summary>Gets a value that represents the number zero (0).</summary>
		public static T Zero;
		/// <summary>Gets a value that represents the number one (1).</summary>
		public static T One;
		/// <summary>Gets a value that represents the number two (2).</summary>
		public static T Two;

		private static MethodInfo _memberwiseCloneFunction = null;
		private static IFormatProvider _formatProvider = null;

		static GenericArithmetic()
		{
			_formatProvider = CultureInfo.CurrentCulture;

			MinusOne = GenericArithmetic<T>.Parse("-1");
			Zero = GenericArithmetic<T>.Parse("0");
			One = GenericArithmetic<T>.Parse("1");
			Two = GenericArithmetic<T>.Parse("2");
		}

		/// <summary>
		/// Converts the specified value.
		/// </summary>
		/// <typeparam name="TFrom">The type of the t from.</typeparam>
		/// <param name="value">The value.</param>
		/// <returns>T.</returns>
		public static T Convert<TFrom>(TFrom value)
		{
			if (typeof(T) == typeof(Complex))
			{
				return (T)((object)new Complex((double)System.Convert.ChangeType(value, typeof(double)), 0d));
			}
			return ConvertImplementation<TFrom, T>.Convert(value);
		}

		/// <summary>
		/// Adds two values and returns the result.
		/// </summary>
		/// <param name="augend">The first value to add.</param>
		/// <param name="addend">The second value to add.</param>
		/// <returns>The sum of the augend and addend.</returns>
		public static T Add(T augend, T addend)
		{
			return GenericArithmeticFactory<T>.GetBinaryExpression(ExpressionType.Add).Invoke(augend, addend);
		}

		/// <summary>
		/// Subtracts one value from another and returns the result.
		/// </summary>
		/// <param name="minuend">The value to subtract from.</param>
		/// <param name="subtrahend">The value to subtract.</param>
		/// <returns>The difference of the minuend and the subtrahend.</returns>
		public static T Subtract(T minuend, T subtrahend)
		{
			return GenericArithmeticFactory<T>.GetBinaryExpression(ExpressionType.Subtract).Invoke(minuend, subtrahend);
		}

		/// <summary>
		/// Returns the product of two values.
		/// </summary>
		/// <param name="multiplicand">The first number to multiply.</param>
		/// <param name="multiplier">The second number to multiply.</param>
		/// <returns>The product of the multiplicand and the multiplier.</returns>
		public static T Multiply(T multiplicand, T multiplier)
		{
			return GenericArithmeticFactory<T>.GetBinaryExpression(ExpressionType.Multiply).Invoke(multiplicand, multiplier);
		}

		/// <summary>
		/// Returns the quotient of two values.
		/// </summary>
		/// <param name="dividend">The number that is being divided.</param>
		/// <param name="divisor">The number by which to divide.</param>
		/// <returns>The quotient of the dividend divided by the divisor.</returns>
		public static T Divide(T dividend, T divisor)
		{
			return GenericArithmeticFactory<T>.GetBinaryExpression(ExpressionType.Divide).Invoke(dividend, divisor);
		}

		/// <summary>
		/// Returns the remainder of a dividend divided by a modulus.
		/// </summary>
		/// <param name="dividend">The number that is being divided.</param>
		/// <param name="modulus">The number by which to divide.</param>
		/// <returns>The remainder of the dividend divided by the modulus.</returns>
		public static T Modulo(T dividend, T modulus)
		{
			return GenericArithmeticFactory<T>.GetBinaryExpression(ExpressionType.Modulo).Invoke(dividend, modulus);
		}

		/// <summary>
		/// Raises a base number to an exponent and returns the result.
		/// </summary>
		/// <param name="base">The number to raise to the exponent power.</param>
		/// <param name="exponent">The exponent to raise the base by.</param>
		/// <returns>The result of raising the base to the exponent.</returns>
		public static T Power(T @base, int exponent)
		{
			Func<T, int, T> powerIntFunc = GenericArithmeticFactory<T>.CreatePowerIntFunction();
			if (powerIntFunc != null)
			{
				return powerIntFunc.Invoke(@base, exponent);
			}
			else
			{
				return Power(@base, ConvertImplementation<int, T>.Convert(exponent));
			}
		}

		/// <summary>
		/// Raises a base number to an exponent and returns the result.
		/// </summary>
		/// <param name="base">The number to raise to the exponent power.</param>
		/// <param name="exponent">The exponent to raise the base by.</param>
		/// <returns>The result of raising the base to the exponent.</returns>
		public static T Power(T @base, T exponent)
		{
			return GenericArithmeticFactory<T>.CreatePowerFunction().Invoke(@base, exponent);
		}

		/// <summary>
		/// Negates the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>T.</returns>
		public static T Negate(T value)
		{
			return Multiply(value, MinusOne);
		}

		/// <summary>
		/// Increments the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>T.</returns>
		public static T Increment(T value)
		{
			return Add(value, One);
		}

		/// <summary>
		/// Decrements the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>T.</returns>
		public static T Decrement(T value)
		{
			return Subtract(value, One);
		}

		/// <summary>
		/// Greaters the than.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public static bool GreaterThan(T left, T right)
		{
			if (ComplexHelperMethods.IsComplexValueType(typeof(T)))
			{
				return ComplexHelperMethods.ComplexEqualityOperator(left, right, ExpressionType.GreaterThan);
			}
			return GenericArithmeticFactory<T>.GetComparisonExpression(ExpressionType.GreaterThan).Invoke(left, right);
		}

		/// <summary>
		/// Lesses the than.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public static bool LessThan(T left, T right)
		{
			if (ComplexHelperMethods.IsComplexValueType(typeof(T)))
			{
				return ComplexHelperMethods.ComplexEqualityOperator(left, right, ExpressionType.LessThan);
			}
			return GenericArithmeticFactory<T>.GetComparisonExpression(ExpressionType.LessThan).Invoke(left, right);
		}

		/// <summary>
		/// Greaters the than or equal.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public static bool GreaterThanOrEqual(T left, T right)
		{
			return (GreaterThan(left, right) || Equal(left, right));
		}

		/// <summary>
		/// Lesses the than or equal.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public static bool LessThanOrEqual(T left, T right)
		{
			return (LessThan(left, right) || Equal(left, right));
		}

		/// <summary>
		/// Equals the specified left.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public static bool Equal(T left, T right)
		{
			if (left == null)
			{
				return (right == null);
			}
			return GenericArithmeticFactory<T>.GetComparisonExpression(ExpressionType.Equal).Invoke(left, right);
		}

		/// <summary>
		/// Nots the equal.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public static bool NotEqual(T left, T right)
		{
			return !Equal(left, right);
		}

		/// <summary>
		/// Squares the root.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>T.</returns>
		public static T SquareRoot(T input)
		{
			Type typeFromHandle = typeof(T);
			if (GenericArithmeticCommon.IsArithmeticValueType(typeFromHandle) && (typeFromHandle != typeof(double)))
			{
				return (T)System.Convert.ChangeType(GenericArithmetic<double>.SquareRoot(System.Convert.ToDouble(input)), typeFromHandle);
			}
			return GenericArithmeticFactory<T>.CreateSqrtFunction().Invoke(input);
		}

		/// <summary>
		/// Mods the pow.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="exponent">The exponent.</param>
		/// <param name="modulus">The modulus.</param>
		/// <returns>T.</returns>
		public static T ModPow(T value, int exponent, T modulus)
		{
			return GenericArithmeticFactory<T>.CreateModPowFunction().Invoke(value, exponent, modulus);
		}

		/// <summary>
		/// Truncates the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>T.</returns>
		public static T Truncate(T input)
		{
			return GenericArithmeticFactory<T>.CreateTruncateFunction().Invoke(input);
		}

		/// <summary>
		/// Parses the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>T.</returns>
		public static T Parse(string input)
		{
			return GenericArithmeticFactory<T>.CreateParseFunction().Invoke(input);
		}

		/// <summary>
		/// Determines the maximum of the parameters.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>T.</returns>
		public static T Max(T left, T right)
		{
			if (GreaterThanOrEqual(left, right))
			{
				return left;
			}
			return right;
		}

		/// <summary>
		/// Abses the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>T.</returns>
		public static T Abs(T input)
		{
			return GenericArithmeticFactory<T>.CreateAbsFunction().Invoke(input);
		}

		/// <summary>
		/// Signs the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>System.Int32.</returns>
		public static int Sign(T input)
		{
			if (GreaterThan(input, Zero))
			{
				return 1;
			}
			else if (LessThan(input, Zero))
			{
				return -1;
			}
			return 0;
		}

		/// <summary>
		/// Divs the rem.
		/// </summary>
		/// <param name="dividend">The dividend.</param>
		/// <param name="divisor">The divisor.</param>
		/// <param name="remainder">The remainder.</param>
		/// <returns>T.</returns>
		public static T DivRem(T dividend, T divisor, out T remainder)
		{
			T rem = Modulo(dividend, divisor);
			remainder = rem;
			return Divide(dividend, divisor);
		}

		/// <summary>
		/// Clones the specified object.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>T.</returns>
		public static T Clone(T obj)
		{
			if (_memberwiseCloneFunction == null)
			{
				_memberwiseCloneFunction = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
			}
			return (T)_memberwiseCloneFunction.Invoke(obj, null);
		}

		/// <summary>
		/// GCDs the specified array.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <returns>T.</returns>
		public static T GCD(IEnumerable<T> array)
		{
			return array.Aggregate(GCD);
		}

		/// <summary>
		/// GCDs the specified left.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns>T.</returns>
		public static T GCD(T left, T right)
		{
			if (ComplexHelperMethods.IsComplexValueType(typeof(T))
				|| typeof(T).Name.Contains("BigDecimal", StringComparison.OrdinalIgnoreCase))
			{
				return ComplexHelperMethods.ModuloFreeGCD(left, right);
			}
			else
			{
				T absLeft = Abs(left);
				T absRight = Abs(right);
				while (NotEqual(absLeft, Zero) && NotEqual(absRight, Zero))
				{
					if (GreaterThan(absLeft, absRight))
					{
						absLeft = Modulo(absLeft, absRight);
					}
					else
					{
						absRight = Modulo(absRight, absLeft);
					}
				}
				return Max(absLeft, absRight);
			}
		}

		/// <summary>
		/// Logs the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="baseValue">The base value.</param>
		/// <returns>T.</returns>
		public static T Log(T value, double baseValue)
		{
			return GenericArithmeticFactory<T>.CreateLogFunction().Invoke(value, baseValue);
		}

		/// <summary>
		/// Returns all divisors of an integer, including 1 and itself.
		/// </summary>
		public static List<T> GetAllDivisors(T value)
		{
			if (IsFloatingPointType(typeof(T)))
			{
				return GetAllDivisors_IntegerImpl(value);
			}

			T n = value;

			if (Equal(Abs(n), One))
			{
				return new List<T> { n };
			}

			List<T> results = new List<T>();
			if (Sign(n) == -1)
			{
				results.Add(MinusOne);
				n = Multiply(n, MinusOne);
			}

			for (T i = One; LessThan(Multiply(i, i), n); i = Increment(i))
			{
				if (Equal(Modulo(n, i), Zero))
				{
					results.Add(i);
				}
			}

			for (T i = SquareRoot(n); GreaterThanOrEqual(i, One); i = Decrement(i))
			{
				if (Equal(Modulo(n, i), Zero))
				{
					results.Add(Divide(n, i));
				}
			}
			return results;
		}

		/// <summary>
		/// Gets all divisors integer implementation.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>List&lt;T&gt;.</returns>
		private static List<T> GetAllDivisors_IntegerImpl(T value)
		{
			int n = ConvertImplementation<T, int>.Convert(value);

			List<int> results = new List<int>();
			if (Math.Abs(n) == 1)
			{
				results.Add(n);
			}
			else
			{
				if (Math.Sign(n) == -1)
				{
					results.Add(-1);
					n = n * -1;
				}

				for (int i = 1; i * i < n; i++)
				{
					if (GenericArithmetic<int>.Modulo(n, i) == 0)
					{
						results.Add(i);
					}
				}

				for (int i = GenericArithmetic<int>.SquareRoot(n); i >= 1; i--)
				{
					if (GenericArithmetic<int>.Modulo(n, i) == 0)
					{
						results.Add(n / i);
					}
				}
			}

			return results.Select(i => ConvertImplementation<int, T>.Convert(i)).ToList();
		}

		/// <summary>
		/// Determines whether [is whole number] [the specified value].
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if [is whole number] [the specified value]; otherwise, <c>false</c>.</returns>
		public static bool IsWholeNumber(T value)
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle.IsGenericType && typeFromHandle.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				typeFromHandle = typeFromHandle.GetGenericArguments()[0];
			}
			TypeCode typeCode = GenericArithmeticCommon.GetTypeCode(typeof(T));
			uint typeCodeValue = (uint)typeCode;

			if (typeFromHandle == typeof(BigInteger))
			{
				return true;
			}
			else if (typeCodeValue > 2 && typeCodeValue < 14) // Is between Boolean and Single
			{
				return true;
			}
			else if (typeCode == TypeCode.Double || typeCode == TypeCode.Decimal)
			{
				return Equal(Modulo(value, One), Zero);
			}
			else if (typeFromHandle == typeof(Complex))
			{
				Complex? complexNullable = value as Complex?;
				if (complexNullable.HasValue)
				{
					Complex complexValue = complexNullable.Value;
					return (complexValue.Imaginary == 0 && complexValue.Real % 1 == 0);
				}
			}
			//else if (type == typeof(BigRational)) { }
			//else if (type == typeof(BigComplex)) { }

			return false;
		}

		/// <summary>
		/// Determines whether [is floating point type] [the specified type].
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns><c>true</c> if [is floating point type] [the specified type]; otherwise, <c>false</c>.</returns>
		public static bool IsFloatingPointType(Type type)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = type.GetGenericArguments()[0];
			}
			TypeCode typeCode = GenericArithmeticCommon.GetTypeCode(typeof(T));

			if (typeCode == TypeCode.Single || typeCode == TypeCode.Double || typeCode == TypeCode.Decimal)
			{
				return true;
			}

			//if (type.Name.Contains("Decimal", StringComparison.OrdinalIgnoreCase)) // BigDecimal
			//{
			//	return true;
			//}

			return false;
		}

		/// <summary>
		/// Determines whether [is integer type] [the specified type].
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns><c>true</c> if [is integer type] [the specified type]; otherwise, <c>false</c>.</returns>
		public static bool IsIntegerType(Type type)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = type.GetGenericArguments()[0];
			}
			TypeCode typeCode = GenericArithmeticCommon.GetTypeCode(typeof(T));
			uint typeCodeValue = (uint)typeCode;

			if (type == typeof(BigInteger))
			{
				return true;
			}
			else if (typeCodeValue >= 5 && typeCodeValue <= 12) // Is between SByte and UInt64
			{
				return true;
			}
			else if (typeCode == TypeCode.Double || typeCode == TypeCode.Decimal)
			{
				return false;
			}
			else if (ComplexHelperMethods.IsComplexValueType(type))
			{
				return false;
			}
			else if (type.Name.Contains("Rational", StringComparison.OrdinalIgnoreCase) // BigRational
					|| type.Name.Contains("Decimal", StringComparison.OrdinalIgnoreCase) // BigDecimal
					|| type.Name.Contains("Fraction", StringComparison.OrdinalIgnoreCase) // Fraction
					|| type.Name.Contains("Float", StringComparison.OrdinalIgnoreCase)) // BigFloat
			{
				return false;
			}
			else if (type.Name.Contains("Integer", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			return false; // ???
		}

		/// <summary>
		/// Determines whether [is fractional value] [the specified value].
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if [is fractional value] [the specified value]; otherwise, <c>false</c>.</returns>
		public static bool IsFractionalValue(T value)
		{
			return (GenericArithmeticCommon.IsArithmeticValueType(value.GetType()) && !IsWholeNumber(value));
		}

		/// <summary>
		/// Squares the root internal.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>T.</returns>
		internal static T SquareRootInternal(T input)
		{
			if (Equal(input, Zero)) { return Zero; }

			T n = Zero;
			T p = Zero;
			T low = Zero;
			T high = Abs(input);

			while (GreaterThan(high, Increment(low)))
			{
				n = Divide(Add(high, low), Two);
				p = Multiply(n, n);

				if (LessThan(input, p)) { high = n; }
				else if (GreaterThan(input, p)) { low = n; }
				else { break; }
			}
			return Equals(input, p) ? n : low;
		}

		/// <summary>
		/// Mods the pow internal.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="exponent">The exponent.</param>
		/// <param name="modulus">The modulus.</param>
		/// <returns>T.</returns>
		internal static T ModPowInternal(T value, int exponent, T modulus)
		{
			T power = Power(value, exponent);
			return Modulo(power, modulus);
		}

		/// <summary>
		/// Converts to bytes.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>System.Byte[].</returns>
		public static byte[] ToBytes(T input)
		{
			Type typeFromHandle = typeof(T);
			if (GenericArithmeticCommon.IsArithmeticValueType(typeFromHandle))
			{
				return GenericArithmeticFactory<T>.CreateValueTypeToBytesFunction().Invoke(input);
			}
			else
			{
				return GenericArithmeticFactory<T>.CreateToBytesFunction(input).Invoke();
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public static string ToString(T input)
		{
			return ToString(input, _formatProvider);
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// Allows passing in some custom number format tokens.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="numberDecimalSeparator">A string that will be used as the decimal point.</param>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		internal static string ToString(T input, IFormatProvider formatProvider)
		{
			NumberFormatInfo numberFormatInfo = (NumberFormatInfo)formatProvider.GetFormat(typeof(NumberFormatInfo));
			char zeroSymbol = numberFormatInfo.NativeDigits[0][0];

			string result = input.ToString();

			IFormattable formattable = input as IFormattable;
			if (formattable != null)
			{
				result = formattable.ToString("G", formatProvider);
			}

			// If there is a decimal point present
			if (result.Contains(numberFormatInfo.CurrencyDecimalSeparator))
			{
				result = result.TrimEnd(zeroSymbol); // Trim all trailing zeros			
				if (result.EndsWith(numberFormatInfo.CurrencyDecimalSeparator)) // If all we are left with is a decimal point
				{
					result = result.TrimEnd(numberFormatInfo.CurrencyDecimalSeparator.ToCharArray()); // Then remove it
				}
			}
			return result;
		}

		/// <summary>
		/// Class ConvertImplementation.
		/// </summary>
		/// <typeparam name="TFrom">The type of the t from.</typeparam>
		/// <typeparam name="TTo">The type of the t to.</typeparam>
		private static class ConvertImplementation<TFrom, TTo>
		{
			private static Func<TFrom, TTo> _convertFunction = null;

			public static TTo Convert(TFrom value)
			{
				if (_convertFunction == null)
				{
					_convertFunction = CreateConvertFunction();
				}
				return _convertFunction.Invoke(value);
			}

			private static Func<TFrom, TTo> CreateConvertFunction()
			{
				ParameterExpression value = Expression.Parameter(typeof(TFrom), "value");
				Expression convert = Expression.Convert(value, typeof(TTo));
				Func<TFrom, TTo> result = Expression.Lambda<Func<TFrom, TTo>>(convert, value).Compile();
				return result;
			}
		}

	}
}
