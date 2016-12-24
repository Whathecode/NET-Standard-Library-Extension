using System;
using System.Reflection;


namespace Whathecode.System.Reflection
{
	public static partial class Extensions
	{
		/// <summary>
		/// Determines whether the reference of the specified <see cref="object" /> equals that from this
		/// when it's a reference type, or whether the values equal when it's a boxed value.
		/// </summary>
		/// <param name = "source">The source for this extension method.</param>
		/// <param name = "o">The object to compare with.</param>
		public static bool ReferenceOrBoxedValueEquals( this object source, object o )
		{
			return ((o != null) && o.GetType().GetTypeInfo().IsValueType)
				? source.Equals( o )
				: source == o;
		}

		/// <summary>
		/// Returns the value of a given member of an object.
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <param name = "member">The member of the object to get the value from.</param>
		/// <returns>The value of the member of the object.</returns>
		public static object GetValue( this object source, MemberInfo member )
		{
			object value;
			if ( member is FieldInfo field )
			{
				value = field.GetValue( source );
			}
			else if ( member is PropertyInfo property )
			{
				value = property.GetValue( source, null );
			}
			else
			{
				throw new NotImplementedException( "Can't return value of the given member." );
			}

			return value;
		}

		/// <summary>
		/// Returns the value of the object at the given location inside this object. If no path is given the object itself is returned.
		/// TODO: Support more advanced property paths. (e.g., indexers) Create custom PropertyPath class?
		/// </summary>
		/// <param name = "source">The source of this extension method.</param>
		/// <param name = "path">
		/// The path in the object to find the value for.
		/// The dot operator can be used to access composed members as you would ordinarily use it.
		/// E.g., Member.SubMember.SubSubMember
		/// </param>
		/// <exception cref = "ArgumentException">Thrown when an invalid path is passed and no value could be found.</exception>
		/// <returns>The object at the given path.</returns>
		public static object GetValue( this object source, string path )
		{
			if ( string.IsNullOrEmpty( path ) )
			{
				return source;
			}

			string[] paths = path.Split( '.' );
			object currentObject = source;

			foreach ( string subPath in paths )
			{
				TypeInfo type = currentObject.GetType().GetTypeInfo();
				MemberInfo[] matchingMembers = type.GetMember( subPath );
				if ( matchingMembers.Length != 1 )
				{
					throw new ArgumentException( "Invalid path \"" + path + "\" for object of type \"" + source.GetType() + "\".", "path" );
				}
				currentObject = currentObject.GetValue( matchingMembers[ 0 ] );
			}

			return currentObject;
		}
	}
}