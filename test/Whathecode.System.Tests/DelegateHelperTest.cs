using System;
using System.Linq.Expressions;
using System.Reflection;
using Whathecode.System;
using Xunit;


namespace Whathecode.Tests.System
{
	/// <summary>
	/// Unit tests for <see cref="DelegateHelper" />.
	/// </summary>
	public class DelegateHelperTest
	{
		#region Common Test Members

		const string FeedMethodName = "Feed";
		const string PlayWithMethodName = "PlayWith";
		const string RunAwayMethodName = "RunAway";

		Pet<Dog> _dog = Pet<Dog>.Domesticate( new Dog() );


		abstract class AbstractAnimal
		{
			public bool Hungry { get; set; }
			public bool Happy { get; set; }

			protected AbstractAnimal()
			{
				Hungry = true;
				Happy = false;
			}
		}


		class Dog : AbstractAnimal {}


		class Bulldog : Dog {}


		class Cat : AbstractAnimal {}


		interface IPet<out T>
		{
			T RunAway();
		}


		class Pet<T> : IPet<T>
			where T : AbstractAnimal, new()
		{
			readonly T _animal;

			Pet( T animal )
			{
				_animal = animal;
			}

			public static Pet<T> Domesticate( T animal )
			{
				return new Pet<T>( animal );
			}

			public void Feed() {}

			public void PlayWith( T friend )
			{
				_animal.Happy = true;
			}

			public T RunAway()
			{
				// No longer a pet!
				return _animal;
			}
		}

		#endregion  // Common Test Members


		[Fact]
		public void WithoutReflectionDelegateTest()
		{
			IPet<Dog> test = Pet<Dog>.Domesticate( new Dog() );
			IPet<AbstractAnimal> animal = test;

			// Covarience for generic parameters in .NET 4.0
			Func<Dog> looseDog = _dog.RunAway;
			Func<AbstractAnimal> anyAnimal = looseDog; // After all, a dog which isn't a pet anymore is not just a dog, but also an animal.
			Func<AbstractAnimal> oldFunc = () => looseDog(); // Prior to .NET 4.0
			anyAnimal();
			oldFunc();

			// Contravariance for generic parameters in .NET 4.0
			Action<Dog> play = _dog.PlayWith;
			Action<Bulldog> bullDogPlay = play; // After all, if dogs can get along, bulldogs can get along as well.
			Action<Bulldog> oldAction = friend => play( friend ); // Prior to .NET 4.0
			bullDogPlay( new Bulldog() );
			oldAction( new Bulldog() );

			// Upcasting, so the specific type is known. Force contravariance for one type.
			Func<Dog> assumeDog = () => (Dog)anyAnimal();
			assumeDog();
			assumeDog = () => (Dog)(AbstractAnimal)new Cat();
			Assert.Throws<InvalidCastException>( () => assumeDog() );

			// Upcasting, so the specific type doesn't need to be known. Force covariance for one type.
			Action<AbstractAnimal> assumeDogAction = d => play( (Dog)d );
			assumeDogAction( new Dog() );
			Assert.Throws<InvalidCastException>( () => assumeDogAction( new Cat() ) );
		}

		[Fact]
		public void OrdinaryCreateDelegateTest()
		{
			// Action, no template arguments, so no problem.
			MethodInfo feedMethod = _dog.GetType().GetMethod( FeedMethodName );
			Action feed = (Action)feedMethod.CreateDelegate( typeof( Action ), _dog );
			feed();

			// Func<T> with known type, and covariance.
			MethodInfo runAwayMethod = _dog.GetType().GetMethod( RunAwayMethodName );
			Func<Dog> runAway = (Func<Dog>)runAwayMethod.CreateDelegate( typeof( Func<Dog> ), _dog );
			Func<AbstractAnimal> covariant
				= (Func<AbstractAnimal>)runAwayMethod.CreateDelegate( typeof( Func<AbstractAnimal> ), _dog );
			runAway();
			covariant();

			// Action<T> with known type, and contraviance.
			MethodInfo playMethod = _dog.GetType().GetMethod( PlayWithMethodName );
			Action<Dog> playWithDog = (Action<Dog>)playMethod.CreateDelegate( typeof( Action<Dog> ), _dog );
			Action<Bulldog> playWithBulldog = (Action<Bulldog>)playMethod.CreateDelegate( typeof( Action<Bulldog> ), _dog );
			playWithDog( new Dog() );
			playWithBulldog( new Bulldog() );

			// Upcasting, so the specific type doesn't need to be known. Force covariance for one type.
			Assert.Throws<ArgumentException>( () => playMethod.CreateDelegate( typeof( Action<AbstractAnimal> ), _dog ) );
		}

		[Fact]
		public void WrapDelegateTest()
		{
			// Parameter and return type wrapping.
			Func<int, int, int> add = ( a, b ) => a + b;
			Func<object, object, object> addWrapped = DelegateHelper.WrapDelegate<Func<object, object, object>>( add );
			Assert.Equal( 10, addWrapped( 5, 5 ) );

			// One-level recursive delegate return type wrapping.
			Func<int, Func<int>> addFive = a => () => a + 5;
			Func<object, Func<object>> addFiveWrapped = DelegateHelper.WrapDelegate<Func<object, Func<object>>>( addFive );
			Assert.Equal( 10, addFiveWrapped( 5 )() );

			// Dynamically generated methods.
			Expression<Func<int, int, int>> expression = ( a, b ) => a + b;
			Func<int, int, int> addDynamic = expression.Compile();
			Func<object, object, object> addDynamicWrapped = DelegateHelper.WrapDelegate<Func<object, object, object>>( addDynamic );
			Assert.Equal( 10, addDynamicWrapped( 5, 5 ) );
		}

		[Fact]
		public void CreateDowncastingDelegateTest()
		{
			// Downcasting, so the specific type doesn't need to be known. Force covariance for one type.
			MethodInfo playMethod = _dog.GetType().GetMethod( PlayWithMethodName );
			Action<AbstractAnimal> play
				= DelegateHelper.CreateDelegate<Action<AbstractAnimal>>( playMethod, _dog, DelegateHelper.CreateOptions.Downcasting );

			// No need to know about the exact type during reflection! As long as you are sure it is the right object.
			play( new Dog() );
			Assert.Throws<InvalidCastException>( () => play( new Cat() ) );
		}

		[Fact]
		public void OrdinaryDynamicInstanceCreateDelegateTest()
		{
			// Action
			MethodInfo feedMethod = _dog.GetType().GetMethod( FeedMethodName );
			Action<Pet<Dog>> feed = (Action<Pet<Dog>>)feedMethod.CreateDelegate( typeof( Action<Pet<Dog>> ) );
			feed( _dog );

			// Action<T>
			MethodInfo playWithMethod = _dog.GetType().GetMethod( PlayWithMethodName );
			Action<Pet<Dog>, Dog> playWith = (Action<Pet<Dog>, Dog>)playWithMethod.CreateDelegate( typeof( Action<Pet<Dog>, Dog> ) );
			playWith( _dog, new Dog() );

			// Func<T>
			MethodInfo runAwayMethod = _dog.GetType().GetMethod( RunAwayMethodName );
			Func<Pet<Dog>, Dog> runAway = (Func<Pet<Dog>, Dog>)runAwayMethod.CreateDelegate( typeof( Func<Pet<Dog>, Dog> ) );
			runAway( _dog );
		}

		[Fact]
		public void CreateDowncastingDynamicInstanceDelegateTest()
		{
			// Downcasting, so the specific type doesn't need to be known. Force covariance for one type.
			MethodInfo playMethod = _dog.GetType().GetMethod( PlayWithMethodName );
			Action<object, AbstractAnimal> play = DelegateHelper.CreateOpenInstanceDelegate<Action<object, AbstractAnimal>>(
				playMethod, DelegateHelper.CreateOptions.Downcasting );

			// No need to know about the exact type during reflection! As long as you are sure it is the right object.
			play( _dog, new Dog() );
			Assert.Throws<InvalidCastException>( () => play( _dog, new Cat() ) );
		}
	}
}