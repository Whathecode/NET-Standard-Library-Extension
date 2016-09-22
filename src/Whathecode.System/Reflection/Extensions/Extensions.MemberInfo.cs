using System;
using System.Linq;
using System.Reflection;


namespace Whathecode.System.Reflection.Extensions
{
	public static partial class Extensions
	{
		/// <summary>
		/// Returns the type of a member when it's a property, field, event or inner class.
		/// </summary>
		/// <param name = "member">The member to get the type from.</param>
		public static Type GetMemberType( this MemberInfo member )
		{
			Type memberType = null;

			switch ( member.MemberType )
			{
				case MemberTypes.Field:
					FieldInfo field = (FieldInfo)member;
					memberType = field.FieldType;
					break;
				case MemberTypes.Property:
					PropertyInfo property = (PropertyInfo)member;
					memberType = property.PropertyType;
					break;
				case MemberTypes.Event:
					EventInfo e = (EventInfo)member;
					memberType = e.EventHandlerType;
					break;
				case MemberTypes.NestedType:
					TypeInfo type = (TypeInfo)member;
					memberType = type.AsType();
					break;
				case MemberTypes.Constructor:
				case MemberTypes.Method:
					throw new InvalidOperationException( "Can't return the type for methods and constructors." );
				default:
					throw new NotSupportedException( "Can't return type of the given member of type \"" + member.GetType() + "\"" );
			}

			return memberType;
		}

		/// <summary>
		/// Get the attributes of the specified type.
		/// </summary>
		/// <typeparam name = "T">The type of the attributes to find.</typeparam>
		/// <param name = "member">The member on which to look for attributes.</param>
		/// <param name = "inherit">Specifies whether to search this member's inheritance chain to find the attributes.</param>
		/// <returns>The found attributes.</returns>
		public static T[] GetAttributes<T>( this MemberInfo member, bool inherit = false )
		{
			Type requestedType = typeof( T );
			if ( !requestedType.GetTypeInfo().IsSubclassOf( typeof( Attribute ) ) )
			{
				throw new ArgumentException( "The requested Attribute type is not an Attribute." );
			}

			return member.GetCustomAttributes( requestedType, inherit ).Cast<T>().ToArray();
		}
	}
}