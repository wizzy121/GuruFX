﻿using System;
using System.Diagnostics;
using GuruFX.Core;
using GuruFX.Core.Entities;

namespace FactoryBenchmark
{
	internal class Program
	{
		public interface ITestObject
		{
			int Value { get; set; }
		}

		public class TestObject : ITestObject
		{
			public TestObject()
			{

			}

			public TestObject(string name)
			{
				Name = name;
			}

			public TestObject(int val)
			{
				Value = val;
			}

			public int Value { get; set; } = 0;

			public string Name { get; set; } = string.Empty;
		}

		private static void Main(string[] args)
		{
			const int warmupIterations = 10000;
			const int iterations = 5000000;

			Console.WriteLine($"Warmup Iterations: {warmupIterations}");
			Console.WriteLine($"Iterations: {iterations}");
			Console.WriteLine();

			RunNewTestObjectBenchmark(warmupIterations, iterations);
			Console.WriteLine();
			RunNewGameObjectBenchmark(warmupIterations, iterations);
			Console.WriteLine();

			RunNewTestObjectBenchmark<TestObject>(warmupIterations, iterations);
			Console.WriteLine();
			RunNewGameObjectBenchmark<GameObject>(warmupIterations, iterations);
			Console.WriteLine();

			RunNewTestObjectFuncBenchmark(warmupIterations, iterations, () => new TestObject());
			Console.WriteLine();
			RunNewGameObjectFuncBenchmark(warmupIterations, iterations, () => new GameObject());
			Console.WriteLine();

			RunTestObjectFactoryBenchmark(warmupIterations, iterations);
			Console.WriteLine();
			RunGameObjectFactoryBenchmark(warmupIterations, iterations);
			Console.WriteLine();

			RunTestObjectActivatorBenchmark(warmupIterations, iterations);
			Console.WriteLine();
			RunGameObjectActivatorBenchmark(warmupIterations, iterations);
			Console.WriteLine();
		}

		private static void RunNewTestObjectBenchmark<T>(int warmupIterations, int iterations) where T : ITestObject, new()
		{
			// warmup
			int sum = 0;
			for (int j = 0; j < warmupIterations; j++)
			{
				T a = new T();
				sum += a.Value;
			}

			// bench it
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int j = 0; j < iterations; j++)
			{
				T a = new T();
				sum += a.Value;
			}
			stopwatch.Stop();

			Console.WriteLine($"Allocate Generic Type: {typeof(T).FullName,40} : {stopwatch.Elapsed}");
		}

		private static void RunNewGameObjectBenchmark<T>(int warmupIterations, int iterations) where T : IEntity, new()
		{
			// warmup
			int sum = 0;
			for (int j = 0; j < warmupIterations; j++)
			{
				T a = new T();
				sum += a.Name.Length;
			}

			// bench it
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int j = 0; j < iterations; j++)
			{
				T a = new T();
				sum += a.Name.Length;
			}
			stopwatch.Stop();

			Console.WriteLine($"Allocate Generic Type: {typeof(T).FullName,40} : {stopwatch.Elapsed}");
		}

		private static void RunNewTestObjectBenchmark(int warmupIterations, int iterations)
		{
			// warmup
			int sum = 0;
			for (int j = 0; j < warmupIterations; j++)
			{
				TestObject a = new TestObject();
				sum += a.Value;
			}

			// bench it
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int j = 0; j < iterations; j++)
			{
				TestObject a = new TestObject();
				sum += a.Value;
			}
			stopwatch.Stop();

			Console.WriteLine($"Allocate Type: {typeof(TestObject).FullName,40} : {stopwatch.Elapsed}");
		}

		private static void RunNewGameObjectBenchmark(int warmupIterations, int iterations)
		{
			// warmup
			int sum = 0;
			for (int j = 0; j < warmupIterations; j++)
			{
				GameObject a = new GameObject();
				sum += a.Name.Length;
			}

			// bench it
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int j = 0; j < iterations; j++)
			{
				GameObject a = new GameObject();
				sum += a.Name.Length;
			}
			stopwatch.Stop();

			Console.WriteLine($"Allocate Type: {typeof(GameObject).FullName,40} : {stopwatch.Elapsed}");
		}

		private static void RunNewTestObjectFuncBenchmark(int warmupIterations, int iterations, Func<TestObject> func)
		{
			// warmup
			int sum = 0;
			for (int j = 0; j < warmupIterations; j++)
			{
				TestObject a = func();
				sum += a.Value;
			}

			// bench it
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int j = 0; j < iterations; j++)
			{
				TestObject a = func();
				sum += a.Value;
			}
			stopwatch.Stop();

			Console.WriteLine($"Allocate with Func Type: {typeof(TestObject).FullName,40} : {stopwatch.Elapsed}");
		}

		private static void RunNewGameObjectFuncBenchmark(int warmupIterations, int iterations, Func<Entity> func)
		{
			// warmup
			int sum = 0;
			for (int j = 0; j < warmupIterations; j++)
			{
				Entity a = func();
				sum += a.Name.Length;
			}

			// bench it
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int j = 0; j < iterations; j++)
			{
				Entity a = func();
				sum += a.Name.Length;
			}
			stopwatch.Stop();

			Console.WriteLine($"Allocate with Func Type: {typeof(Entity).FullName,40} : {stopwatch.Elapsed}");
		}

		private static void RunTestObjectActivatorBenchmark(int warmupIterations, int iterations)
		{
			Activator<string, ITestObject> testObjectActivator = new Activator<string, ITestObject>();

			Type t1 = typeof(TestObject);

			//BenchmarkTestObjectActivator(t1, (k) => testObjectActivator.ActivateInstance(k), warmupIterations, iterations);
			BenchmarkTestObjectActivator(t1, (k) => testObjectActivator.ActivateInstance(k, "Fred"), warmupIterations, iterations);
		}

		private static void RunGameObjectActivatorBenchmark(int warmupIterations, int iterations)
		{
			Activator<string, IEntity> gameObjectActivator = new Activator<string, IEntity>();

			Type e1 = typeof(GameObject);

			//BenchmarkGameObjectActivator(e1, (k) => gameObjectActivator.ActivateInstance(k), warmupIterations, iterations);
			BenchmarkGameObjectActivator(e1, (k) => gameObjectActivator.ActivateInstance(k, "Fred"), warmupIterations, iterations);
		}

		private static void RunTestObjectFactoryBenchmark(int warmupIterations, int iterations)
		{
			Factory<string, ITestObject> testObjectFactory = new Factory<string, ITestObject>();

			testObjectFactory.Register("Object_FactoryFunc", () => new TestObject());
			testObjectFactory.Register("Object_FactoryFunc_WithArgs", () => new TestObject("Fred"));
			testObjectFactory.Register("Object_CompiledExpression(id, type)", typeof(TestObject));

			string[] creationMethods =
			{
				"Object_FactoryFunc",
				"Object_FactoryFunc_WithArgs",
				"Object_CompiledExpression(id, type)",
			};

			foreach (string creationMethod in creationMethods)
			{
				BenchmarkTestObjectCreationMethod(warmupIterations, testObjectFactory, creationMethod, iterations);
			}
		}

		private static void RunGameObjectFactoryBenchmark(int warmupIterations, int iterations)
		{
			Factory<string, IEntity> entityFactory = new Factory<string, IEntity>();

			entityFactory.Register("Entity_FactoryFunc", () => new GameObject());
			entityFactory.Register("Entity_FactoryFunc_WithArgs", () => new GameObject("Fred"));
			entityFactory.Register("Entity_CompiledExpression(id, type)", typeof(GameObject));

			string[] creationMethods =
			{
				"Entity_FactoryFunc",
				"Entity_FactoryFunc_WithArgs",
				"Entity_CompiledExpression(id, type)",
			};

			foreach (string creationMethod in creationMethods)
			{
				BenchmarkGameObjectCreationMethod(warmupIterations, entityFactory, creationMethod, iterations);
			}
		}

		private static void BenchmarkTestObjectCreationMethod(int warmupIterations, Factory<string, ITestObject> factory, string creationMethod, int iterations)
		{
			// warmup
			int sum = 0;
			for (int j = 0; j < warmupIterations; j++)
			{
				ITestObject a = factory.Create(creationMethod);
				sum += a.Value;
			}

			// bench it
			sum = 0;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int j = 0; j < iterations; j++)
			{
				ITestObject a = factory.Create(creationMethod);
				sum += a.Value;
			}
			stopwatch.Stop();

			Console.WriteLine($"{creationMethod,40} : {stopwatch.Elapsed}");
		}

		private static void BenchmarkGameObjectCreationMethod(int warmupIterations, Factory<string, IEntity> factory, string creationMethod, int iterations)
		{
			// warmup
			int sum = 0;
			for (int j = 0; j < warmupIterations; j++)
			{
				IEntity a = factory.Create(creationMethod);
				sum += a.Name.Length;
			}

			// bench it
			sum = 0;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int j = 0; j < iterations; j++)
			{
				IEntity a = factory.Create(creationMethod);
				sum += a.Name.Length;
			}
			stopwatch.Stop();

			Console.WriteLine($"{creationMethod,40} : {stopwatch.Elapsed}");
		}

		private static void BenchmarkGameObjectActivator<TKeyType, TValueType>(TKeyType keyToActivate, Func<TKeyType, TValueType> factoryFunc, int warmupIterations, int iterations) where TValueType : IEntity
		{
			// warmup
			int sum = 0;
			for (int j = 0; j < warmupIterations; j++)
			{
				TValueType a = factoryFunc(keyToActivate);
				sum += a.Name.Length;
			}

			// bench it
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int j = 0; j < iterations; j++)
			{
				TValueType a = factoryFunc(keyToActivate);
				sum += a.Name.Length;
			}
			stopwatch.Stop();

			Console.WriteLine($"Activate Type: {keyToActivate.ToString(),40} : {stopwatch.Elapsed}");
		}

		private static void BenchmarkTestObjectActivator<TItemType, TValueType>(TItemType itemTypeToActivate, Func<TItemType, TValueType> factoryFunc, int warmupIterations, int iterations) where TValueType : ITestObject
		{
			// warmup
			int sum = 0;
			for (int j = 0; j < warmupIterations; j++)
			{
				TValueType a = factoryFunc(itemTypeToActivate);
				sum += a.Value;
			}

			// bench it
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int j = 0; j < iterations; j++)
			{
				TValueType a = factoryFunc(itemTypeToActivate);
				sum += a.Value;
			}
			stopwatch.Stop();

			Console.WriteLine($"Activate Type: {itemTypeToActivate.ToString(),40} : {stopwatch.Elapsed}");
		}
	}
}
