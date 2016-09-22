using System;
using System.Linq.Expressions;
using UnaryOperation
	= System.Func<System.Linq.Expressions.Expression, System.Linq.Expressions.UnaryExpression>;
using BinaryOperation
	= System.Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression, System.Linq.Expressions.BinaryExpression>;
using BinaryOperationMethod
	= System.Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression, System.Reflection.MethodInfo, System.Linq.Expressions.BinaryExpression>;
using System.Reflection;


namespace Whathecode.System.Operators
{
	/// <summary>
	/// Helper class to construct operators for the generic operator classes.
	/// </summary>
	/// <remarks>
	/// Based on work by Marc Gravell contributed to the MiscUtils library and the Arithmetic library written by Rüdiger Klaehn.
	/// </remarks>
	static class OperatorHelper
	{
		/// <summary>
		/// Compile a delegate which performs a unary operation.
		/// </summary>
		/// <typeparam name = "TArg">The type of the argument.</typeparam>
		/// <typeparam name = "TResult">The type of the result.</typeparam>
		/// <param name = "operation">The unary operation to perform.</param>
		public static Func<TArg, TResult> CompileUnaryExpression<TArg, TResult>( UnaryOperation operation )
		{
			ParameterExpression arg = Expression.Parameter( typeof( TArg ), "arg" );

			return Expression.Lambda<Func<TArg, TResult>>( operation( arg ), arg ).Compile();
		}

		/// <summary>
		/// Compile a delegate which performs a binary operation.
		/// </summary>
		/// <typeparam name = "TArg1">The type of the first argument.</typeparam>
		/// <typeparam name = "TArg2">The type of the second argument.</typeparam>
		/// <typeparam name = "TResult">The type of the result.</typeparam>
		/// <param name="operation">The binary operation to perform.</param>
		public static Func<TArg1, TArg2, TResult> CompileBinaryExpression<TArg1, TArg2, TResult>( BinaryOperation operation )
		{
			ParameterExpression arg1 = Expression.Parameter( typeof( TArg1 ), "arg1" );
			ParameterExpression arg2 = Expression.Parameter( typeof( TArg2 ), "arg2" );

			return Expression.Lambda<Func<TArg1, TArg2, TResult>>( operation( arg1, arg2 ), arg1, arg2 ).Compile();
		}

		/// <summary>
		/// Compile a delegate which performs a binary operation using a custom specified method.
		/// </summary>
		/// <typeparam name = "TArg1">The type of the first argument.</typeparam>
		/// <typeparam name = "TArg2">The type of the second argument.</typeparam>
		/// <typeparam name = "TResult">The type of the result.</typeparam>
		/// <param name = "operation">The binary operation to perform.</param>
		/// <param name = "method">The method to use for the binary operation.</param>
		public static Func<TArg1, TArg2, TResult> CompileBinaryExpression<TArg1, TArg2, TResult>( BinaryOperationMethod operation, MethodInfo method )
		{
			ParameterExpression arg1 = Expression.Parameter( typeof( TArg1 ), "arg1" );
			ParameterExpression arg2 = Expression.Parameter( typeof( TArg2 ), "arg2" );

			return Expression.Lambda<Func<TArg1, TArg2, TResult>>( operation( arg1, arg2, method ), arg1, arg2 ).Compile();
		}
	}
}
