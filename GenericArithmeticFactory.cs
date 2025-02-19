using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Globalization;

namespace ExtendedArithmetic
{
	/// <summary>
	/// Static class with static methods for obtaining delegates to arithmetic operations.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class GenericArithmeticFactory<T>
	{
		/// <summary>Gets a value that represents the number negative one (-1).</summary>
		public readonly static T MinusOne;
		/// <summary>Gets a value that represents the number zero (0).</summary>
		public readonly static T Zero;
		/// <summary>Gets a value that represents the number one (1).</summary>
		public readonly static T One;
		/// <summary>Gets a value that represents the number two (2).</summary>
		public readonly static T Two;

		private static string _numberDecimalSeparator = null;

		private static Dictionary<string, Func<T, T>> _unaryExpressionDictionary;
		private static Dictionary<ExpressionType, Func<T, T, T>> _binaryExpressionDictionary;
		private static Dictionary<ExpressionType, Func<T, T, bool>> _comparisonExpressionDictionary;

		private static Func<T, int, T> _powerIntFunction = null;
		private static Func<T, T, T, T> _modpowFunction = null;
		private static Func<T, double, T> _logFunction = null;
		private static Func<T, byte[]> _getBytesFunction = null;
		private static Func<byte[]> _toBytesArrayFunction = null;

		static GenericArithmeticFactory()
		{
			_unaryExpressionDictionary = new Dictionary<string, Func<T, T>>();
			_binaryExpressionDictionary = new Dictionary<ExpressionType, Func<T, T, T>>();
			_comparisonExpressionDictionary = new Dictionary<ExpressionType, Func<T, T, bool>>();

			_numberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			MinusOne = GenericArithmetic<T>.Parse("-1");
			Zero = GenericArithmetic<T>.Parse("0");
			One = GenericArithmetic<T>.Parse("1");
			Two = GenericArithmetic<T>.Parse("2");
		}

		/// <summary>
		/// Returns a delegate that calls the appropriate unary operation.
		/// An expression is built that performs the requested operation,
		/// it is then compiled into a method body, and a delegate to that method is returned.
		/// The compiled expressions are cached in Dictionary and returned on subsequent calls.
		/// </summary>
		/// <param name="operationType">The unary expression the delegate should perform.</param>
		/// <returns>A delegate that performs the selected operation when invoked.</returns>
		/// <exception cref="System.NotSupportedException">ExpressionType not supported: {Enum.GetName(typeof(ExpressionType), operationType)}.</exception>
		internal static Func<T, T> GetUnaryExpression(ExpressionType operationType)
		{
			if (_unaryExpressionDictionary.ContainsKey(Enum.GetName(typeof(ExpressionType), operationType)))
			{
				return _unaryExpressionDictionary[Enum.GetName(typeof(ExpressionType), operationType)];
			}

			ParameterExpression value = Expression.Parameter(typeof(T), "value");

			UnaryExpression operation = null;
			if (operationType == ExpressionType.Negate)
			{
				operation = Expression.Negate(value);
			}
			else
			{
				throw new NotSupportedException($"ExpressionType not supported: {Enum.GetName(typeof(ExpressionType), operationType)}.");
			}

			Func<T, T> result = Expression.Lambda<Func<T, T>>(operation, value).Compile();
			_unaryExpressionDictionary.Add(Enum.GetName(typeof(ExpressionType), operationType), result);
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls the appropriate binary arithmetic operation overload.
		/// An expression is built that performs the requested operation,
		/// it is then compiled into a method body, and a delegate to that method is returned.
		/// The compiled expressions are cached in Dictionary and returned on subsequent calls.
		/// </summary>
		/// <param name="operationType">The binary expression the delegate should perform.</param>
		/// <returns>A delegate that performs the selected operation when invoked.</returns>
		/// <exception cref="System.NotSupportedException">ExpressionType not supported: {Enum.GetName(typeof(ExpressionType), operationType)}.</exception>
		public static Func<T, T, T> GetBinaryExpression(ExpressionType operationType)
		{
			if (_binaryExpressionDictionary.ContainsKey(operationType))
			{
				return _binaryExpressionDictionary[operationType];
			}

			ParameterExpression left = Expression.Parameter(typeof(T), "left");
			ParameterExpression right = Expression.Parameter(typeof(T), "right");

			BinaryExpression operation = null;
			if (operationType == ExpressionType.Add) { operation = Expression.Add(left, right); }
			else if (operationType == ExpressionType.Subtract) { operation = Expression.Subtract(left, right); }
			else if (operationType == ExpressionType.Multiply) { operation = Expression.Multiply(left, right); }
			else if (operationType == ExpressionType.Divide) { operation = Expression.Divide(left, right); }
			else if (operationType == ExpressionType.Modulo) { operation = Expression.Modulo(left, right); }
			else if (operationType == ExpressionType.Power) { operation = Expression.Power(left, right); }
			else
			{
				throw new NotSupportedException($"ExpressionType not supported: {Enum.GetName(typeof(ExpressionType), operationType)}.");
			}

			Func<T, T, T> result = Expression.Lambda<Func<T, T, T>>(operation, left, right).Compile();
			_binaryExpressionDictionary.Add(operationType, result);
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls the appropriate comparison operation overloads.
		/// An expression is built that performs the requested operation,
		/// it is then compiled into a method body, and a delegate to that method is returned.
		/// The compiled expressions are cached in Dictionary and returned on subsequent calls.
		/// </summary>
		/// <param name="operationType">The comparison expression the delegate should perform.</param>
		/// <returns>A delegate that performs the selected operation when invoked.</returns>
		public static Func<T, T, bool> GetComparisonExpression(ExpressionType operationType)
		{
			if (_comparisonExpressionDictionary.ContainsKey(operationType))
			{
				return _comparisonExpressionDictionary[operationType];
			}

			ParameterExpression left = Expression.Parameter(typeof(T), "left");
			ParameterExpression right = Expression.Parameter(typeof(T), "right");

			BinaryExpression comparison = null;
			if (operationType == ExpressionType.GreaterThan) { comparison = Expression.GreaterThan(left, right); }
			else if (operationType == ExpressionType.LessThan) { comparison = Expression.LessThan(left, right); }
			else if (operationType == ExpressionType.GreaterThanOrEqual) { comparison = Expression.GreaterThanOrEqual(left, right); }
			else if (operationType == ExpressionType.LessThanOrEqual) { comparison = Expression.LessThanOrEqual(left, right); }
			else if (operationType == ExpressionType.Equal) { comparison = Expression.Equal(left, right); }
			else if (operationType == ExpressionType.NotEqual) { comparison = Expression.NotEqual(left, right); }

			Func<T, T, bool> result = Expression.Lambda<Func<T, T, bool>>(comparison, left, right).Compile();
			_comparisonExpressionDictionary.Add(operationType, result);
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls the Parse method on the type T.
		/// </summary>
		/// <exception cref="System.NotSupportedException">Cannot find public static method 'Parse' for type of {typeFromHandle.FullName}.</exception>
		public static Func<string, T> CreateParseFunction()
		{
			Type typeFromHandle = typeof(T);

			MethodInfo[] methods = typeFromHandle.GetMethods(BindingFlags.Static | BindingFlags.Public);
			var filteredMethods =
				methods.Where(
					mi => mi.Name == "Parse"
					&& mi.GetParameters().Count() == 1
					&& mi.GetParameters().First().ParameterType == typeof(string)
				);

			MethodInfo method = null;
			if (typeFromHandle == typeof(Complex))
			{
				method = typeof(ComplexHelperMethods).GetMethod("Parse", BindingFlags.Static | BindingFlags.Public);
			}
			else
			{
				method = filteredMethods.FirstOrDefault();
			}

			if (method == null)
			{

				throw new NotSupportedException($"Cannot find public static method 'Parse' for type of {typeFromHandle.FullName}.");
			}

			ParameterExpression parameter = Expression.Parameter(typeof(string), "input");
			MethodCallExpression methodCall = Expression.Call(method, parameter);
			Func<string, T> result = Expression.Lambda<Func<string, T>>(methodCall, parameter).Compile();
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls Math.Sqrt for .NET value types or
		/// calls an internal Sqrt implementation that can operate on the generic type T.
		/// </summary>
		public static Func<T, T> CreateSqrtFunction()
		{
			if (_unaryExpressionDictionary.ContainsKey(nameof(Math.Sqrt)))
			{
				return _unaryExpressionDictionary[nameof(Math.Sqrt)];
			}

			MethodInfo method;
			Type typeFromHandle = typeof(T);
			if (GenericArithmeticCommon.IsArithmeticValueType(typeFromHandle))
			{
				method = typeof(Math).GetMethod("Sqrt", BindingFlags.Static | BindingFlags.Public);
			}
			else
			{
				method = typeFromHandle.GetMethod("Sqrt", BindingFlags.Static | BindingFlags.Public);
			}

			if (method == null)
			{
				return GenericArithmetic<T>.SquareRootInternal;
			}

			ParameterExpression parameter = Expression.Parameter(typeFromHandle, "input");
			MethodCallExpression methodCall = Expression.Call(method, parameter);
			Func<T, T> result = Expression.Lambda<Func<T, T>>(methodCall, parameter).Compile();
			_unaryExpressionDictionary.Add(nameof(Math.Sqrt), result);
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls Math.Sign for .NET value types,
		/// fails otherwise
		/// </summary>
		public static Func<T, int> CreateSignFunction()
		{
			//if (_unaryExpressionDictionary.ContainsKey(nameof(Math.Sign)))
			//{
			//	return _unaryExpressionDictionary[nameof(Math.Sign)];
			//}

			MethodInfo method;
			Type typeFromHandle = typeof(T);
			if (GenericArithmeticCommon.IsArithmeticValueType(typeFromHandle))
			{
				method = typeof(Math).GetMethod("Sign", BindingFlags.Static | BindingFlags.Public);
			}
			else
			{
				throw new NotImplementedException();
			}

			ParameterExpression parameter = Expression.Parameter(typeFromHandle, "value");
			MethodCallExpression methodCall = Expression.Call(method, parameter);
			Func<T, int> result = Expression.Lambda<Func<T, int>>(methodCall, parameter).Compile();
			//_unaryExpressionDictionary.Add(nameof(Math.Sign), result);
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls Math.Abs for .NET value types
		/// or the static method T.Abs on the type T, if it exists.
		/// </summary>
		/// <exception cref="System.NotSupportedException">Cannot find public static method 'Abs' for type of {typeFromHandle.FullName}.</exception>
		public static Func<T, T> CreateAbsFunction()
		{
			if (_unaryExpressionDictionary.ContainsKey(nameof(Math.Abs)))
			{
				return _unaryExpressionDictionary[nameof(Math.Abs)];
			}

			Type typeFromHandle = typeof(T);

			ParameterExpression value = Expression.Parameter(typeFromHandle, "value");
			MethodInfo method = null;
			Expression methodCall_AutoConversion = null;

			if (GenericArithmeticCommon.IsArithmeticValueType(typeFromHandle))
			{
				var methods = typeof(Math).GetMethods(BindingFlags.Static | BindingFlags.Public);
				var absMethods = methods.Where(mi => mi.Name == "Abs");
				absMethods = absMethods.Where(mi => mi.GetParameters()[0].ParameterType == typeFromHandle);
				method = absMethods.FirstOrDefault();

				if (method == null)
				{
					throw new NotSupportedException($"Cannot find public static method 'Abs' for type of {typeFromHandle.FullName}.");
				}

				methodCall_AutoConversion = Expression.Call(method, value);
			}
			else
			{
				var methods = typeFromHandle.GetMethods(BindingFlags.Static | BindingFlags.Public);
				var absMethods = methods.Where(mi => mi.Name == "Abs").ToList();
				method = absMethods.FirstOrDefault();

				if (method == null)
				{
					throw new NotSupportedException($"Cannot find public static method 'Abs' for type of {typeFromHandle.FullName}.");
				}

				Expression methodCallExpression = Expression.Call(method, value);
				methodCall_AutoConversion = GenericArithmeticCommon.ConvertIfNeeded(methodCallExpression, typeFromHandle);
			}

			Func<T, T> result = Expression.Lambda<Func<T, T>>(methodCall_AutoConversion, value).Compile();
			_unaryExpressionDictionary.Add(nameof(Math.Abs), result);
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls Math.Truncate for .NET value types
		/// or the static method T.Truncate on the type T, if it exists.
		/// </summary>
		public static Func<T, T> CreateTruncateFunction()
		{
			if (_unaryExpressionDictionary.ContainsKey(nameof(Math.Truncate)))
			{
				return _unaryExpressionDictionary[nameof(Math.Truncate)];
			}

			MethodInfo method = null;
			Type typeFromHandle = typeof(T);

			if (typeFromHandle == typeof(double) || typeFromHandle == typeof(decimal))
			{
				method = typeof(Math).GetMethod("Truncate", new Type[] { typeof(T) });
			}
			else
			{
				method = typeFromHandle.GetMethod("Truncate", new Type[] { typeof(T) });
			}

			if (method == null)
			{
				return new Func<T, T>((arg) => arg);
			}

			ParameterExpression parameter = Expression.Parameter(typeFromHandle, "input");
			MethodCallExpression methodCall = Expression.Call(method, parameter);
			Func<T, T> result = Expression.Lambda<Func<T, T>>(methodCall, parameter).Compile();
			_unaryExpressionDictionary.Add(nameof(Math.Truncate), result);
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls Math.Pow for .NET value types
		/// or the static method T.Pow on the type T, if it exists.
		/// </summary>
		/// <exception cref="System.NotSupportedException">Cannot find public static method 'Pow' for type of {typeFromHandle.FullName}.</exception>
		public static Func<T, T, T> CreatePowerFunction()
		{
			if (_binaryExpressionDictionary.ContainsKey(ExpressionType.Power))
			{
				return _binaryExpressionDictionary[ExpressionType.Power];
			}

			Type typeFromHandle = typeof(T);

			ParameterExpression baseVal = Expression.Parameter(typeFromHandle, "baseValue");
			ParameterExpression exponent = Expression.Parameter(typeFromHandle, "exponent");

			MethodInfo method = null;
			Expression methodCall_AutoConversion = null;

			if (GenericArithmeticCommon.IsArithmeticValueType(typeFromHandle))
			{
				method = typeof(Math).GetMethod("Pow", BindingFlags.Static | BindingFlags.Public);
			}
			else
			{
				var methods = typeFromHandle.GetMethods(BindingFlags.Static | BindingFlags.Public);
				var powMethods = methods.Where(mi => mi.Name == "Pow").ToList();
				method = powMethods.FirstOrDefault();
			}

			if (method != null)
			{
				Expression exponent_AutoConversion = null;
				if (typeFromHandle == typeof(BigInteger))
				{
					exponent_AutoConversion = Expression.Convert(exponent, typeof(int), typeof(GenericArithmetic<T>).GetMethod("ConvertBigIntegerToInt", BindingFlags.Static | BindingFlags.NonPublic));

					methodCall_AutoConversion = Expression.Call(method, baseVal, exponent_AutoConversion);
				}
				else
				{

					Type returnType = method.ReturnType;

					ParameterInfo baseParameterInfo = method.GetParameters()[0];
					ParameterInfo expParameterInfo = method.GetParameters()[1];

					Expression baseVal_AutoConversion = GenericArithmeticCommon.ConvertIfNeeded(baseVal, baseParameterInfo.ParameterType);
					exponent_AutoConversion = GenericArithmeticCommon.ConvertIfNeeded(exponent, expParameterInfo.ParameterType);

					Expression methodCallExpression = Expression.Call(method, baseVal_AutoConversion, exponent_AutoConversion);

					methodCall_AutoConversion = GenericArithmeticCommon.ConvertIfNeeded(methodCallExpression, typeFromHandle);
				}
			}

			if (method == null || methodCall_AutoConversion == null)
			{
				throw new NotSupportedException($"Cannot find public static method 'Pow' for type of {typeFromHandle.FullName}.");
			}

			Func<T, T, T> result = Expression.Lambda<Func<T, T, T>>(methodCall_AutoConversion, baseVal, exponent).Compile();
			_binaryExpressionDictionary.Add(ExpressionType.Power, result);
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls the Math.Pow overload that takes an integer exponent parameter.
		/// </summary>
		/// <exception cref="System.NotSupportedException">Cannot find public static method 'Pow' for type of {typeFromHandle.FullName} who's exponent is assignable from int.</exception>
		public static Func<T, int, T> CreatePowerIntFunction()
		{
			Type typeFromHandle = typeof(T);
			if (_powerIntFunction != null)
			{
				return _powerIntFunction;
			}

			ParameterExpression baseVal = Expression.Parameter(typeFromHandle, "baseValue");
			ParameterExpression exponent = Expression.Parameter(typeof(int), "exponent");

			MethodInfo method = null;
			Expression methodCall_AutoConversion = null;

			if (GenericArithmeticCommon.IsArithmeticValueType(typeFromHandle))
			{
				method = typeof(Math).GetMethod("Pow", BindingFlags.Static | BindingFlags.Public);
			}
			else
			{
				var methods = typeFromHandle.GetMethods(BindingFlags.Static | BindingFlags.Public);
				var powMethods = methods.Where(mi => mi.Name == "Pow").ToList();

				var expAssignable = powMethods.Where(mi => mi.GetParameters()[1].ParameterType.IsAssignableFrom(typeof(int)));

				method = powMethods.FirstOrDefault();
			}

			if (method != null)
			{
				Type returnType = method.ReturnType;

				ParameterInfo baseParameterInfo = method.GetParameters()[0];
				ParameterInfo expParameterInfo = method.GetParameters()[1];

				Expression baseVal_AutoConversion = GenericArithmeticCommon.ConvertIfNeeded(baseVal, baseParameterInfo.ParameterType);
				Expression exponent_AutoConversion = GenericArithmeticCommon.ConvertIfNeeded(exponent, expParameterInfo.ParameterType);

				Expression methodCallExpression = Expression.Call(method, baseVal_AutoConversion, exponent_AutoConversion);

				methodCall_AutoConversion = GenericArithmeticCommon.ConvertIfNeeded(methodCallExpression, typeFromHandle);
			}

			if (method == null || methodCall_AutoConversion == null)
			{
				throw new NotSupportedException($"Cannot find public static method 'Pow' for type of {typeFromHandle.FullName} who's exponent is assignable from int.");
			}

			Func<T, int, T> result = Expression.Lambda<Func<T, int, T>>(methodCall_AutoConversion, baseVal, exponent).Compile();
			_powerIntFunction = result;
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls the static method T.ModPow method on the type T, if it exists.
		/// </summary>
		public static Func<T, T, T, T> CreateModPowFunction()
		{
			Type typeFromHandle = typeof(T);
			if (_modpowFunction != null)
			{
				return _modpowFunction;
			}

			MethodInfo method = typeFromHandle.GetMethod("ModPow", BindingFlags.Static | BindingFlags.Public);
			if (method == null)
			{
				return GenericArithmetic<T>.ModPowInternal;
			}

			ParameterExpression val = Expression.Parameter(typeFromHandle, "value");
			ParameterExpression exp = Expression.Parameter(typeFromHandle, "exponent");
			ParameterExpression mod = Expression.Parameter(typeFromHandle, "modulus");
			MethodCallExpression methodCall = Expression.Call(method, val, exp, mod);
			Func<T, T, T, T> result = Expression.Lambda<Func<T, T, T, T>>(methodCall, val, exp, mod).Compile();
			_modpowFunction = result;
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls Math.Log for .NET value types
		/// or the static method T.Log on the type T, if it exists.
		/// </summary>
		/// <exception cref="System.NotSupportedException">No such method 'Log' on type {typeFromHandle.FullName}.</exception>
		public static Func<T, double, T> CreateLogFunction()
		{
			MethodInfo[] methods = null;
			if (_logFunction != null)
			{
				return _logFunction;
			}

			Type typeFromHandle = typeof(T);
			if (GenericArithmeticCommon.IsArithmeticValueType(typeFromHandle))
			{
				methods = typeof(Math).GetMethods(BindingFlags.Static | BindingFlags.Public);
			}
			else
			{
				methods = typeFromHandle.GetMethods(BindingFlags.Static | BindingFlags.Public);
			}

			var filteredMethods = methods.Where(mi => mi.Name == "Log" && mi.GetParameters().Count() == 2);
			MethodInfo method = filteredMethods.FirstOrDefault();
			if (method == null)
			{
				throw new NotSupportedException($"No such method 'Log' on type {typeFromHandle.FullName}.");
			}

			ParameterExpression val = Expression.Parameter(typeFromHandle, "value");

			ParameterInfo valueParameterInfo = method.GetParameters()[0];
			Expression value_AutoConversion = GenericArithmeticCommon.ConvertIfNeeded(val, valueParameterInfo.ParameterType);

			ParameterExpression baseVal = Expression.Parameter(typeof(double), "baseValue");
			MethodCallExpression methodCallExpression = Expression.Call(method, value_AutoConversion, baseVal);

			Expression methodCall_AutoConversion = GenericArithmeticCommon.ConvertIfNeeded(methodCallExpression, typeFromHandle);

			Func<T, double, T> result = Expression.Lambda<Func<T, double, T>>(methodCall_AutoConversion, val, baseVal).Compile();
			_logFunction = result;
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls the BitConverter.GetBytes method.
		/// </summary>
		internal static Func<T, byte[]> CreateValueTypeToBytesFunction()
		{
			Type typeFromHandle = typeof(T);
			if (_getBytesFunction != null)
			{
				return _getBytesFunction;
			}

			var allMethods = typeof(BitConverter).GetMethods(BindingFlags.Static | BindingFlags.Public);
			var matchingNameMethods = allMethods.Where(mi => mi.Name == "GetBytes");
			var matchingTypeMethods = matchingNameMethods.Where(mi => mi.GetParameters()[0].ParameterType == typeFromHandle);
			MethodInfo method = matchingTypeMethods.FirstOrDefault();
			ParameterExpression parameter = Expression.Parameter(typeFromHandle, "input");
			MethodCallExpression methodCall = Expression.Call(method, parameter);
			Func<T, byte[]> result = Expression.Lambda<Func<T, byte[]>>(methodCall, parameter).Compile();
			_getBytesFunction = result;
			return result;
		}

		/// <summary>
		/// Returns a delegate that calls the ToByteArray method on the type T.
		/// </summary>
		/// <exception cref="System.NotSupportedException">Cannot find suitable method to convert instance of type {typeFromHandle.FullName} to an array of bytes.</exception>
		internal static Func<byte[]> CreateToBytesFunction(T instanceObject)
		{
			Type typeFromHandle = typeof(T);
			if (_toBytesArrayFunction != null)
			{
				return _toBytesArrayFunction;
			}

			var allMethods = typeFromHandle.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			var matchingMethods = allMethods.Where(mi => mi.Name == "ToByteArray");

			MethodInfo method = matchingMethods.FirstOrDefault();
			if (method == null)
			{
				throw new NotSupportedException($"Cannot find suitable method to convert instance of type {typeFromHandle.FullName} to an array of bytes.");
			}

			MethodCallExpression methodCall = Expression<T>.Call(Expression.Constant(instanceObject), method);
			Func<byte[]> result = Expression.Lambda<Func<byte[]>>(methodCall).Compile();
			_toBytesArrayFunction = result;
			return result;
		}

	}
}
