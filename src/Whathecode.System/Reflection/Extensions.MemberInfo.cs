using System;
using System.Reflection;


namespace Whathecode.System.Reflection
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
	}
}