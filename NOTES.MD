# Building an Async API with ASP.NET Core
Culled from the PluralSight course Building an Async API with ASP.NET Core. 

It contains best practices on how to improve scalability by writing async code, naming guidelines, 
when it makes sense to use async on the server and when it doesn't, integrating with external services 
(including working with multiple calls & dealing with cancellation) and a set of common pitfalls to avoid.  

Built with ASP.NET Core 2.1:

Frameworks and Tooling:
- Visual Studio 2017
- .NET Core 2.1
- West Wind WebSurge (Load testing.)


# Understanding the Power of Async
Why async? Hint: Performance is not the key benefit.
The key benefit of writing async server-side code is increased scalability.
	1. Horizontal Scaling: RESTful APIs are easy to scale because RESTful is stateless. 
		Note: Non-distributed databases and caches hurt scalability.
	2. Vertical Scaling: Memory and storage. Async improves vertical scalability.

Synchronous: One thread via the thread pool per full request. 503 Service unavailable without available threads.
Asynchronous: Thread via the thread pool. Thread is returned to the thread pool per each completed Task<>. API uses service resources in a more efficient manner.

I/O bound work. Will my code be waiting for a task to be complete before continuing? e.g.: File styatem, database, network calls.
Computational-bound work. Will my code be performing an expensive computation? Don't use async on the server for computational-bound work.

Thread: A basic unit of CPU utilization:
	1. Multithreading: One single CPU or single core in a multi-core CPU can execute multiple threads concurrently.
	2. Concurrency: A condition that exists what at least two threads are making process.
	3. Parallelism: At least two threads are executing simultaneously. Achieved with a multi-core processor. Or a single-threaded hypercore processor. (Duh.)

Summary: The thread that's handling an async request is freed up to handle other requests. Use async on the server for I/O -bound work.


# Starting at the bottom with your Data Access Layer
Keywords async and await:
Mark the method with the async modifier.
- Ensure that the await keyword can be used inside the method.
- Transforming the method into a state machine.
Using the await operator:
- Tells the computer that the async method cannot continue unitl the awaited asynchronous process is complete.
- Returns control to the caller of the async method:
	1. A method that is not marked with the async modifier cannot be awaited
	2. When an async method doesn't contain an await operator, the method simply executes as a synchronous method.

Async return types: void, Task, Task<T>, or a type with an accessable GetAwaiter method:
	1. Void: Only advised for even handlers as it is hard to handle exception. Difficult to test. No easy manner in which to notify the calling code about the status.
	2. Task and Task<T>: represents a single operation that returns nothing, Task, or a value of type T, Task<T>, and usually executes asynchronously.
		- Task: Represents the execution of the async method.
	3. Task and Task<T>: Status, IsCanceled, IsCompleted, and IsFaulted properties allow determining the state of a Task.
		- Get status complete. Through Task, we can know the state of an async operation and its result.
	4. GetAwaiter method by which the returned object implements the System.Runtime.CompilerServices.ICriticalNotifyCompletion interface.

Task and Task<T> are refernce types.
Using reference types can induce memory allocation in performance-critical paths, and that can adversely affect performance.
Supporting generalized return types allows returning a lighweight value type.

The purpose of Task and Task<T>
Naming guidlines, conventions, and best practices.

Task-based Asynchronous Pattern: TAP. Best practice today. Based upon Task, Task<T>, GetAwaiter() -implementing types.
Event-based asynchronous pattern. EAP. Multithreading without the complexity. MethodNameAsync (method) MethodNameCompleted (event) MethodNameAsyncCancel (method)
Mainly used before .NET 4
Asynchronous programming model. APM. Two methods named BeginOperatinName and EndOpertaionName.

PM> add-migration InitialLibraryMigration
	- Up method migrates from previous to current
	- Down method executed when we migrate from new version to previous version

Repository pattern: An abstraction that reduces complexity and aims to make the code, safe for the repository implementation, persistence ingorant. ('Persistence ingorant' like my neighbor.)
Contracts and Async modifiers:
	1. An interface is a contract, which makes the GetBooksAsync() definition a contract detail.
	2. Using the await/async keywords tell us how the method is implemented, which makes it an implementation detail.
	Trick: Return await context.Books.ToListAsync().GetAwaiter; '> "GetAwaiter" ensures async compatibilitity. (e.g.: obj.Dispose() ensures IDisposable implementation.)

```csharp
	// This leverages the DBContext. 
	// That being said, we must register the BooksRepository with a lifetime scope equal to, or shorter than, that of the DBContext.

	// AddScoped: Why? EF Core team wanted assurances that the context would be disposed of after every request. 
	// Records are not read across requests, thus ensuring performance.

	services.AddScoped<IBooksRepository, BooksRepository>();
```


# Asynchronously Reading Resources
With a Controller: 
- Derive from ControllerBase and add the [ApiController] attributes at the class-level, thus allowing attribute-based routing which is a best practice.
- We can now use the [Route()] attribute.

WebSurge: Scalability Improvements. 1. Load Testing 2. Thread Pool Throttling
When publishing: Pack deploys new version of the API without attaching the degugger. Attaching a debugger can result in errant load test results.
Set the maximum number of threads to the processor count. Simulate a longer-running process with a "WAIT FOR DELAY" statement at the sql level.

Entity versus DTO:
Entity model classes represent database rows as objects. Outer-facing models represent objects to be sent across the wire.
Note: Do not mix models and responsibilities between layers as it leads to evolability issues.
DTOs represent the data type we'd like to send back.
	- Model classes: DTOs. With something like AutoMapper. Static approach.
	- Using dynamics, anonymous objects, and ExpandoObject. Dynamic approach. (e.g.: adding hypermedia links.)

Either way, we'll be adding mapping code within the controller. We'll be using a reuseable filter: IAsyncResultFilter.
Manipulating output with an AsyncResultFilter:
Filters in ASP.NET Core MVC allow us to run code before or after specific stages in the request processing pipeline.
e.g.:
Request => Delegate Sequence {Middleware => Middleware} => Response.
Keep in mind, we can short circuit the request. e.g.: failed authorization.

ASP.NET Core MVC has its own pipeline that requests are sent through.
Filters run within the MVC action invocation pipeline. (AKA filter pipeline.)

So... Action => Exception Filters => Result Filters => Result Exectes.

ASP.NET Core MVC Filter Pipeline:
	Authorization Filters
	Resource Filters
	MODEL BINDING
	Action Filters
	Exception Filters
	Result Filters (Map)
	Result Executes

Promote reuse. Keep the actions cleaner. IResultFilter, IAsyncResultFilter interfaces. ResultFilterAttribute (abstract.)

ResultFilterAttribute: 'Result' is the result from the Action as ObjectResult.
Note: We can return a result without an object, like NotFound(). We need to ensure that there is something there to map.

AutoMapper. Use AutoMapper.Extensions.Microsoft.DependencyInjection version 4.0.1.
Derive from Profile as a manner in which to organize mapping configurations. It's overkill as used in this code.
Register AutoMapper as a service and you'll scrape all classes derived from :Profile.

Do not violate the single responsibility principle. Thus, do not use the following within the same filter.
```csharp
	if (typeof(IEnumerable).IsAssignableFrom(resultFromAction.Value.GetType())) { };
```
Create the two filters: BookResultFilterAttribute.cs and BooksResultFilterAttribute.cs
Note: When used in the Controller, the name of the attribute can be simplified: [BooksResultFilter()]


# Asynchronously Manipulating Resources
Creating a Resource: 
- Initial design was to AddAsync(). When we add to a context, we are not yet persisting to the db, so it's not I/O-bound.

Bulk Inserts:
- Less overhead. Performance increase. Higher availability of threads. ( Beter scalability.)
	POST api/books: One book.
	POST api/bookcollections: One collection of books. (Treated as a resourse)
Note: Remember to support a location header for the bulk inserts.


# Understanding How To Integrate with External Services Using Async
- Asynchronously Calling an External Service
- Waiting for Multiple Tasks to Complete
- Cancelling Tasks
- Gracefully Handle Exceptions

So, we'll use a Tuple as a backing to create a book with many covers for our filter.
C# 7.0 has a value Tuple structure:
- new Tuple<Book, IEnumerable<BookCover>>(book, covers);
- e.g.: (Book book, IEnumerable<CookCover> covers) propertyBag = (book, bookCovers);

// Tuple in C# 7.0 synax.
// (Entities.Book book, IEnumerable<BookCover> covers) propertyBag = (entity, bookCovers);
// return Ok((book: propertyBag.book, bookCovers: propertyBag.covers));

Tasks in Parallel:
- Code offers a nice example of deferred execution.

Cancellation Matters: Free up threads (I/O bound work.)
- Returns to thread pool and improves scalability.
- Frees up CPU resources (computational bound work.)

TaskCancelledException: Gracefully handle this exception.
await throws, or re-throws, exceptions without wrappering them in an aggregate.

So we need to log and rethrow, because we cannot reliabiliy continue.
- Overarching try... catch { logger.LogError(); throw; }

Execute multipl tasks in order by awaiting them
- Task results are returned in order.
Use Task.WhenAll() or Task.WhenAny() when executing multiple tasks in parallel.
- Task results are returned when the task in done.
Cancel tasks to free up threads.
- CancellationTokenSource && CancellationToken
Handle a cancellation exception by catching it as an OperationCancelledException
- Exposes CancellationToken.


# Avoiding Common Pitfalls
Wrapping Legacy Code:
- Legacy code, like long-running algorithms, is computational bound code.
- These can be offloaded to a background thread using async/await.
- 1. Can run concurrently.

Wrapping synchronous code with Task.Run().

Should we use Task.Run(() => on a server?

Blocking Async Code:

Modifying Shared State:

Using Task.Run() on the Server:
Note: ASP.NET Core in snot optimized for Rask.Run()
- Creates an unoptimized thread and causes overhead.
- Task.Run() on the server decreases scalability.

Great for one scenario: It's intended for use on the client. e.g.: To keep the UI responsive.

Blocking Async code: using .Result. 
- Entities are returned when the task is completed, instead of simply the Task. Which blocks the thread.
- .Wait will  also block the tread and return entities.
- So... Task.Wait() and Task.Result() block the calling thread.
- Blocking async code hurts scalability.

.NET Core does not have a synchronization context. We only use the threadpool.
- Improves poerformance and makes it easier to wirte asunc code.

- Without synchronization context, it is very unsafe to modify shared state.
e.g.: await Task.WhenAll(x,y): Not threadsafe with a List<T>, correct state is not guarenteed if both threads access at the very same time.

- Different threads night manipulate the same state at the same time.
- Correctness cannot be guarenteed.

Three common pitfalls:
1. Do not use Task.Run() on the server.
- Don't block Async code by using .Task.Result() or Thread.Await()
- Do not modify shared state.

I am now ready to be awesome. Cheers!