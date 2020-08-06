# TicTacToeML with Unity ml-agnets
If you are here, the chances are you are still learning machine learning stuff just like myself. I highly encourge you to build this project by yourself before take a look at my code and settings. Because you will learn a lot of things and find questions that you didn't even think about before building it by youself. Those questions will really improve your skills and knowledge about ML. :)

# Settings
Alright first thing is first. Please follow Getting Started page in Unity and make it sure you can play and train the example project.

https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Getting-Started.md

Once you are done with the tutorial, you should be able open this project with no issue.

# Project structure.
There are three Visual Studio 2019 projects here.

1. TicTacToeLib - This is a pure game logic library project.
2. TicTacToeCmd - This is a command line window tic-tac-toe client.
3. TicTacToeUnity - This is a Unity client.

I like to have a seperate game logic project from Unity because it allows me to test things out qucikly without launching Unity. This approach is also very useful if you want to run your game logic in both server and client (Unity). 

./TicTacToe.sln contains TicTacToeLib and TicTacToeCmd projects. ./TicTacToeUnity/TicTacToeUnity.sln cotains TicTacToeUnity. If you are only interested in machine learning part, you can ignore TicTacToeLib and TicTacToeCmd projects.

# Unity
TicTacToeUnity is using Unity 2019.4.4f1. You should be able to open it by choosing ./TicTacToeUnity directory through Unity Hub.

# Play against the trained NN
If you want to play against the neural network model I trained, please open ./
