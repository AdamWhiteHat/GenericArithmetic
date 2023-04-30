using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedArithmetic
{
	public class ComplexComparer<T> : IComparer<T>
	{
		public int Compare(T l, T r)
		{
			Type parameterType = typeof(T);

			// Get the sign of the COMPLEXTYPE.Real property

			int leftSign = ComplexHelperMethods.ComplexGetRealPartSign<T>(l);
			int rightSign = ComplexHelperMethods.ComplexGetRealPartSign<T>(r);

			// Get COMPLEXTYPE.Abs method. Call it.

			var paramMethods = parameterType.GetMethods(BindingFlags.Static | BindingFlags.Public);
			MethodInfo absMethod = paramMethods.Where(mi => mi.Name == "Abs").FirstOrDefault();

			Type returnType = absMethod.ReturnType;
			MethodInfo[] returnMethods = returnType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
			MethodInfo compareToMethod = returnMethods.Where(mi => (mi.Name == "CompareTo"))
													  .Where(mi => mi.GetParameters().FirstOrDefault()?.ParameterType == returnType)
													  .FirstOrDefault();

			ParameterExpression leftParameter = Expression.Parameter(parameterType, "left");
			ParameterExpression rightParameter = Expression.Parameter(parameterType, "right");

			MethodCallExpression leftAbs = Expression.Call(absMethod, leftParameter);
			MethodCallExpression rightAbs = Expression.Call(absMethod, rightParameter);

			// Dynamically invoke call, get Abs call results.

			LambdaExpression leftAbsLambda = Expression.Lambda(leftAbs, leftParameter);
			LambdaExpression rightAbsLambda = Expression.Lambda(rightAbs, rightParameter);
			Delegate leftAbsDelegate = leftAbsLambda.Compile();
			Delegate rightAbsDelegate = rightAbsLambda.Compile();
			object leftAbsResultObj = leftAbsDelegate.DynamicInvoke(l);
			object rightAbsResultObj = rightAbsDelegate.DynamicInvoke(r);

			// Negate Abs results if sign of COMPLEXTYPE.Real was -1

			object leftCorrectedResult = null;
			object rightCorrectedResult = null;
			if (leftSign == -1)
			{
				leftCorrectedResult = ComplexHelperMethods.ComplexNegate(leftAbsResultObj);
			}
			else
			{
				leftCorrectedResult = leftAbsResultObj;
			}

			if (rightSign == -1)
			{
				rightCorrectedResult = ComplexHelperMethods.ComplexNegate(rightAbsResultObj);
			}
			else
			{
				rightCorrectedResult = rightAbsResultObj;
			}

			// Call CompareTo on Abs results.

			ConstantExpression leftConstant = Expression.Constant(leftCorrectedResult, returnType);
			ParameterExpression rightAbsParameter = Expression.Parameter(returnType, "rightAbs");

			MethodCallExpression compareToExp = Expression.Call(leftConstant, compareToMethod, rightAbsParameter);
			LambdaExpression compareToLambda = Expression.Lambda(compareToExp, rightAbsParameter);
			Delegate compareToDelegate = compareToLambda.Compile();

			object resultObj = compareToDelegate.DynamicInvoke(rightCorrectedResult);
			int resultInt = (int)resultObj;
			return resultInt;
		}
	}
}
