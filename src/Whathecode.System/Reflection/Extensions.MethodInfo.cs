using System;
using System.Reflection;


namespace Whathecode.System.Reflection
{
	public static partial class Extensions
	{
		/// <summary>
		/// Returns a <see cref="MethodInfo" /> object based on the original <see cref="MethodInfo" /> object
		/// with its type parameters replaced with the given type arguments.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <param name = "typeArguments">
		/// An array of types to be substituted for the type parameters of the given <see cref="MethodInfo" />.
		/// </param>
		/// <returns>
		/// A <see cref="MethodInfo" /> object that represents the constructed method formed by substituting
		/// the elements of <paramref name="typeArguments" /> for the type parameters of the current method definition.
		/// </returns>
		public static MethodInfo GetGenericMethodDefinition( this MethodInfo source, params Type[] typeArguments )
		{
			if ( !source.IsGenericMethod )
			{
				throw new ArgumentException( "Only a generic method can have its type parameters replaced.", nameof( source ) );
			}

			return source
				.GetGenericMethodDefinition()
				.MakeGenericMethod( typeArguments );
		}
	}
}