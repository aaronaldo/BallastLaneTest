# BallastLaneTest
Technical test for Ballast Lane.

==== PROJECT OVERVIEW ====

The idea of the project is a mini and internal Social Media for a company (in this case, Ballast Lane).
The users may create posts, add comments to any post, and like posts and comments.
Also, the user may edit and delete their posts and comments.

==== ARCHITECTURAL OVERVIEW ====

In this project we use microservices, which will be one for Post and Comments (since they are part of the same context), and one for User context.
The advantage of separating them into different microservices is that we can scalate them individually, since there will be WAY more activity in posts and comments context than in users context.

Each microservice has:
- API project, with Controllers and Requests models
- Infrastructure project, with a Mediator (business layer), Repository and DTOs (when needed). We're using Mediator Pattern for more loosely coupled components.
- Models project, with the models that will be used in database.

For querying, we have the so called "keys" which are combinations for a context filter. It is really useful when caching (even though we're not using cache here yet due to lack of time).

The flow is:
- User --> API (Controller) --> Mediator --> Repository --> Database

We will use Unit Test projects for mediators and repositories, using xUnit framework.

==== DATABASE ====

There is a SQL Scripts folder, with a script for the table creations.
There is a table for Users, Posts, Comments, and 2 N-N for User likes for comments and posts.

==== ADDITIONAL NOTES ====
Not all that is stated here was able to be finished due to an unexpected issue in big part of the weekend, even though I am adding them here to give an idea on how would be the full product with more time.
