Feature: WriteOnlyIndexerGeneration
	I am checking the Write Only Indexer Generation strategy


Scenario: Write Only Indexer Generation
	Given I have a class defined as 
	"""
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public class Form1 
    {
		public string this[string cookieName, Guid cookieValue, DateTime cookieExpirationTime]
        {
            set
            {
            }
        } 

        public string this[string cookieName]
        {
            get {
                return "hello";
            }
        } 

        public string this[string cookieName, int cookieId]
        {
            get {
                return "hello";
            }
			set {
			}			 
        } 
    }
}
	"""
	And I set my test framework to 'NUnit3'
	And I set my mock framework to 'FakeItEasy'
	When I generate tests for the indexer using the strategy 'WriteOnlyIndexerGenerationStrategy'
	Then I expect a method called 'CanSetIndexerForStringAndGuidAndDateTime'
		And I expect it to have the attribute 'Test'
		And I expect it to contain the statement 'Assert.Fail("Create or modify test");'
		And I expect it to contain a statement like '_testClass[{{{AnyString}}}, new Guid({{{AnyString}}}), DateTime.UtcNow] = {{{AnyString}}};'
	And I expect no method with a name like '.*StringAndInt.*'
