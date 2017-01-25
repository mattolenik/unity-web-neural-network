# About

Rollaround is a .NET library implementing a simple multi-layer feed forward neural network with supervised learning.

It's currently just an ad-hoc library for an [AI demo project](https://github.com/mattolenik/AISteeringDemo), and is geared towards use within Unity 3D. Particularly for the WebGL target, which ultimately converts the IL to JavaScript. It uses very few allocations, bare arrays instead of lists, loops instead of iterators, no LINQ, and targets only the .NET 2.0 framework.

I would consider this pre-alpha. It works, but has no tests, probably has edge cases, and could use an API redesign. I did some initial profiling with Unity's built-in profiler, and found that the script execution time was quite reasonable. I was calling FeedForward() on 20 4-layer networks with 400+ weights each, every 1/60th of a second. That CPU time was much lower than the time spent doing physics raycasts for distance checking. So qualitatively it seems pretty good, and will probably be developed further.

## Use case

An AI steering and obstacle avoidance demo using this library.

[![AI obstacle avoidance demo](http://i.imgur.com/5dX7v3L.gif)](https://youtu.be/Ffkjfok1HTY)

The rolling movement of the droids is the inspiration for the project's name.

## Roadmap

1. Write tests
2. Reduce heap allocation, use arrays of structs instead of classes
3. Allow for topologies more complex than just: inputs -> n hidden layers w/ m neurons each -> outputs.
4. Backpropgation learning
5. Whatever its use cases demand
