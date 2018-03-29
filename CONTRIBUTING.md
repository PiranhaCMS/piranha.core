# Contributing to Piranha

First off, thanks for showing interest in contributing to Piranha. Let's make 
this short and sweet so we can all go back to coding awesome new features!

## What is Piranha

Piranha is a **framework** and does **not** contain any components that produce
client code. If you want to contribute such functionality, this is **not** the place 
and you should instead create your own community package.

Piranha is **cross platform**. This means that nothing can be added to the core
packages that creates a dependency on a specific operating system or runtime
environment. If you want to contribute such functionality you should also create
your own community package.

Piranha is **lightweight**. Just because you **can** add a feature doesn't necessarily
mean that you **should**. If you're not sure if your idea fits the project, please open 
an issue to get a second opinion on it.

## Testing

When contributing new functionality, make sure you add the adequate tests
for your code. We want our test coverage to **go up** (or at least stay the same)
with each commit. For testing we use the `xunit` test framework. If you contribute 
functionality to an existing project, please add tests in the matching test project. 
If you contribute by creating a new project, please create a new matching test project.

Integration tests **must** run on `SQLite` so that they can be executed on our
build servers. Possible test data should be seeded before the tests starts and
deleted after the tests are finished so that tests can be run several times
without resetting the environment.

## Adding features

To keep commit logs as clean as possible we use the GitHub workflow with feature
branches. This means:

1. **Never** write any code in your `master` branch
2. When writing code, do it in a specific feature branch
3. Send your pull request from that feature branch back to this repo
4. After your pull request has been accepted, sync the changes into your master from the upstream remote
5. Delete you feature branch
6. Again, **NEVER** write any code in your `master` branch
