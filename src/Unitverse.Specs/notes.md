1. Change the input class to support both string and file / resource input
2. Use a SpecFlow transform to convert a text strategy name to the actual strategy object
  Not sure I can do this, as I need a frameworkset to create and return the strategy. This might work via dependency injection for the context

3. Add full generation option support
  - Added test and mock framework support
  - still plenty of other options to add if we need them
  - Still need to look at the relationship between FrameworkSet and GenerationOptions - feels like we have to
	framework and mock types twice - why? And what happens if they are out of sync?

4. Get the actual generated class as a result, instead of just the methods from the strategy Create call
5. Add framework steps for checking the "shape" of the generated class
  = Added a basic method count assert - should be able to add additional ones as needed.
