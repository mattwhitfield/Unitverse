Feature: CanConstructSingleConstructorGeneration
	I am checking the Can Construct Single Constructor Generation strategy


Scenario: Can Construct Single Constructor Generation
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
	And I set my test framework to 'XUnit'
	And I set my mock framework to 'NSubstitute'
	When I generate unit tests for the class using strategy 'CanConstructSingleConstructorGenerationStrategy'
	Then I expect a method called 'CanConstruct'
		And I expect it to contain 1 statements called 'Assert.NotNull(instance);'