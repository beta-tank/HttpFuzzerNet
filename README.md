# HTTP Fuzzer.Net
Simple HTTP Fuzzer writen on C#. This project is created in the education purposes.
[![Build status](https://ci.appveyor.com/api/projects/status/kv005l3dyxomxob4?svg=true)](https://ci.appveyor.com/project/beta-tank/httpfuzzernet)
![Screenshot](/Common/Screenshot1.png "Application window")

## Dependenses
* [Fare] (https://github.com/moodmosaic/Fare)
* [EntroBuilder] (https://github.com/ymotton/EntroTester)

Libraries connected to project via NuGet. Before build project you must restore it.

## Solution structure
* HttpFuzzer.Gui - fuzzer project
* HttpFuzzer.Server - simple server for testing

## Features
* Send requests async
* Generate URL from ReGex
* Can send GET and POST requests
* Editable User-Agent
* Static or ReGex parameters for request
* Selective response logging to file
