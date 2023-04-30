using System;
using System.Linq.Expressions;

namespace ExtendedArithmetic
{
	public static class GenericArithmeticCommon
	{
		/// <summary>
		/// Determines if the System.Type is a .NET value type.
		/// Value types: Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, and Double.
		/// </summary>
		/// <param name="type">The type to test.</param>
		/// <returns><c>true</c> if the parameter type is a .net value type; otherwise, <c>false</c>.</returns>
		public static bool IsArithmeticValueType(Type type)
		{
			TypeCode typeCode = GetTypeCode(type);
			if ((uint)(typeCode - 7) <= 8u)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Converts a System.Type into a System.TypeCode.
		/// Handles resolving generic types to their concrete type.
		/// </summary>
		/// <param name="fromType">The type to convert.</param>
		/// <returns>A TypeCode that reflects the type of parameter fromType.</returns>
		internal static TypeCode GetTypeCode(Type fromType)
		{
			Type type = fromType;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = type.GetGenericArguments()[0];
			}
			if (type.IsEnum)
			{
				return TypeCode.Object;
			}

			return Type.GetTypeCode(type);
		}

		/// <summary>
		/// Checks the expression's return type 
		/// (accepts both parameter expressions and method call expressions)
		/// and adds an expression to cast the return type as the targetType if they do not match.
		/// </summary>
		/// <param name="valueExpression">The value expression.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <returns>An Expression of the same type that casts the return type to the target type if needed.</returns>
		internal static Expression ConvertIfNeeded(Expression valueExpression, Type targetType)
		{
			Type returnType = null;
			if (valueExpression.NodeType == ExpressionType.Parameter)
			{
				returnType = ((ParameterExpression)valueExpression).Type;
			}
			else if (valueExpression.NodeType == ExpressionType.Call)
			{
				returnType = ((MethodCallExpression)valueExpression).Method.ReturnType;
			}

			if (returnType != targetType)
			{
				return Expression.Convert(valueExpression, targetType);
			}
			return valueExpression;
		}
	}
}
