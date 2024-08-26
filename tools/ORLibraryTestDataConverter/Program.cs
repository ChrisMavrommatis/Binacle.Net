using Binacle.Net.TestsKernel.Models;
using Cocona;
using Newtonsoft.Json;

namespace ORLibraryTestDataConverter
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Cocona.CoconaLiteApp.Run((
				[Option('i')]string input, 
				[Option('o')]string output
				) =>
			{
				// read input file
				using var reader = new System.IO.StreamReader(input);

				var counter = 1;
				var hasNextLine = true;

				// Get the filename without extension as the name prefix for the test cases
				var name = input.Split('.',StringSplitOptions.RemoveEmptyEntries)[0];
				var scenarios = new List<CompactScenario>();
				while (hasNextLine)
				{
					var testCaseLine = reader.ReadLine();
					if(testCaseLine is null)
					{
						hasNextLine = false;
						break;
					}

					// Test Case
					// 1 2502505
					// 1 112 104 88.47 89.52 1
					// 587 233 220
					// 3
					// 1 108 0 76 0 30 1 40
					// 2 110 0 43 1 25 1 33
					// 3 92 1 81 1 55 1 39

					// First line in each test case is an ID.
					var testCaseIds = testCaseLine.Split(' ');

					// Second line states the results of the test, as reported in the EB-AFIT master's thesis, appendix E.
					string[] testResults = reader.ReadLine().Split(' ');
					var totalItems = Convert.ToDecimal(testResults[1]);
					var totalPackedItems = Convert.ToDecimal(testResults[2]);
					var containerVolumePacked = Convert.ToDecimal(testResults[3]);
					var itemsVolumePacked = Convert.ToDecimal(testResults[4]);

					// Third line defines the container dimensions.
					string[] containerDims = reader.ReadLine().Split(' ');

					// Fourth line states how many distinct item types we are packing.
					int itemTypeCount = Convert.ToInt32(reader.ReadLine());

					// compactFormat   
					// q-LxWxH
					var items = new List<string>();

					for (int i = 0; i < itemTypeCount; i++)
					{
						string[] itemArray = reader.ReadLine().Split(' ');
						var length = itemArray[1];
						var width = itemArray[3];
						var height = itemArray[5];
						var quantity = itemArray[7];
						items.Add($"{quantity}-{length}x{width}x{height}");
					}


					var expectedResult = totalItems == totalPackedItems ? "Fit" : "None";
					var scenario = new CompactScenario
					{
						Name = $"{name}_{testCaseIds[1]}",
						BinCollection = $"Raw:{containerDims[0]}x{containerDims[1]}x{containerDims[2]}",
						ExpectedSize = testResults[4],
						Items = items.ToArray()
					};

					scenarios.Add(scenario);
				}
				var json = JsonConvert.SerializeObject(scenarios, Formatting.Indented);

				// write output file
				using var writer = new System.IO.StreamWriter(output);
				
				writer.Write(json);

			});
		}
	}
}
