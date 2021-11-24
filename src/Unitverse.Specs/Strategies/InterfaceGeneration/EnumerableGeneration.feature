Feature: EnumerableGeneration
	I am checking the Enumerable Generation strategy


Scenario: Enumerable Generation
	Given I have a class defined as 
	"""
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
		
namespace WindowsFormsApp1
{
    public class Form1 : IEnumerable<int>
    {
        public Form1()
        {
             
        }
	
		public IEnumerator<int> GetEnumerator()
        {
            return Enumerable.Empty<int>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
	"""
	And I set my test framework to 'NUnit3'
	And I set my mock framework to 'Moq'
	When I generate unit tests for the class using strategy 'EnumerableGenerationStrategy'
	Then I expect a method called 'ImplementsIEnumerable_Int32'
		And I expect it to contain the statement 'Assert.That(actualCount, Is.EqualTo(expectedCount));'
		And I expect it to contain a using statement with a 'while' token