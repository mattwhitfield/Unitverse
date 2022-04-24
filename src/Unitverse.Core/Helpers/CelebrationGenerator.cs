namespace Unitverse.Core.Helpers
{
    using System;
    using System.Linq;

    public static class CelebrationGenerator
    {
        public static string GetAnimal(out string name)
        {
            var rand = new Random();
            var values = new Tuple<string, string>[]
            {
                Tuple.Create("Bernard the unit-testing bear", Animals.Bear),
                Tuple.Create("Charlie the unit-testing cat", Animals.Cat),
                Tuple.Create("Dominic the unit-testing dinosaur", Animals.Dinosaur),
                Tuple.Create("Dave the unit-testing dog", Animals.Dog),
                Tuple.Create("Mike the unit-testing mouse", Animals.Mouse),
                Tuple.Create("Pete the unit-testing pig", Animals.Pig),
                Tuple.Create("Oliver the unit-testing owl", Animals.Owl),
                Tuple.Create("Steve the unit-testing spider", Animals.Spider),
            };

            var animal = values[rand.Next(values.Length)];

            name = animal.Item1;
            return animal.Item2;
        }

        public static string GetMessage(IGenerationStatistics generationStatistics)
        {
            if (generationStatistics is null)
            {
                throw new ArgumentNullException(nameof(generationStatistics));
            }

            var animal = GetAnimal(out var name);

            return GetMessage(animal, name, generationStatistics);
        }

        public static string GetMessage(string animal, string name, IGenerationStatistics generationStatistics)
        {
            if (string.IsNullOrWhiteSpace(animal))
            {
                throw new ArgumentNullException(nameof(animal));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (generationStatistics is null)
            {
                throw new ArgumentNullException(nameof(generationStatistics));
            }

            var lines = animal.Lines().ToList();
            while (lines.Count < 4)
            {
                lines.Add(string.Empty);
            }

            var width = lines.Max(x => x.Length) + 3;
            var paddedLines = lines.Select(x => x.PadRight(width)).ToList();

            var adder = (lines.Count - 4) / 2;

            paddedLines[0 + adder] = paddedLines[0 + adder] + name + " says:";
            paddedLines[1 + adder] = paddedLines[1 + adder] + "C O N G R A T U L A T I O N S !";
            paddedLines[3 + adder] = paddedLines[3 + adder] + "You have created " + generationStatistics.TestMethodsGenerated.ToString("N0") + " test methods with Unitverse!";

            return string.Join("\r\n", paddedLines.Select(x => x.TrimEnd()));
        }
    }
}
