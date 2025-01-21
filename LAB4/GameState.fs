module GameState
open SFML.Window
open SFML.Graphics
open SFML.System
open GameConsts
open System.Diagnostics

type CoinAnimationState = {
    CurrentFrame: int
    FrameTime: float32
}

type Coin = {
    Position: Vector2f 
}

type Platform = {
    Position: Vector2f
    Width: float32
    Height: float32
}

type Enemy = {
    Position: Vector2f
    Size: float32
    Direction: float32 
    Speed: float32
}

type GameState = {
    Position : Vector2f
    VerticalSpeed : float32
    isJumping: bool
    Coins: Coin list
    CollectedCoins: int
    Enemies: Enemy list
    CoinAnimation: CoinAnimationState
}

type Door = {
    Position: Vector2f
    Size: Vector2f
}

let initialCoins: Coin list = [
    { Position = Vector2f(80.0f, 460.0f)}
    { Position = Vector2f(350.0f, 550.0f)}
    { Position = Vector2f(60.0f, 415.0f)}
    { Position = Vector2f(40.0f, 280.0f)}
    { Position = Vector2f(60.0f, 100.0f)}
    { Position = Vector2f(210.0f, 55.0f)}
    { Position = Vector2f(750.0f, 550.0f) }
    { Position = Vector2f(405.0f, 460.0f)}
    { Position = Vector2f(405.0f, 325.0f) }
    { Position = Vector2f(600.0f, 370.0f)}
    { Position = Vector2f(450.0f, 190.0f)}
    { Position = Vector2f(430.0f, 55.0f) }
]

let finishCoins = List.length initialCoins
let platforms = [
    { Position = Vector2f(0.0f, float32 windowHeight - 30.0f); Width = float32 windowWidth; Height = 30.0f } // Нижняя платформа
    { Position = Vector2f(0.0f, 0.0f); Width = float32 windowWidth; Height = 30.0f } // Верхняя платформа
    { Position = Vector2f(0.0f, 30.0f); Width = 30.0f; Height = float32 windowHeight - 60.0f} // Левая полоса
    { Position = Vector2f(float32 windowWidth - 30.0f, 30.0f); Width = 30.0f; Height = float32 windowHeight - 60.0f } // Правая полоса

    { Position = Vector2f(380.0f, 300.0f); Width = 20.0f; Height = 270.0f } // вертикальный столб  
    { Position = Vector2f(220.0f, 525.0f); Width = 160.0f; Height = 20.0f }
    { Position = Vector2f(30.0f, 480.0f); Width = 180.0f; Height = 20.0f }
    { Position = Vector2f(260.0f, 480.0f); Width = 220.0f; Height = 20.0f }
    { Position = Vector2f(30.0f, 435.0f); Width = 300.0f; Height = 20.0f }
    { Position = Vector2f(30.0f, 390.0f); Width = 120.0f; Height = 20.0f }
    { Position = Vector2f(170.0f, 345.0f); Width = 80.0f; Height = 20.0f }
    { Position = Vector2f(30.0f, 300.0f); Width = 100.0f; Height = 20.0f }
    { Position = Vector2f(270.0f, 300.0f); Width = 190.0f; Height = 20.0f }
    { Position = Vector2f(290.0f, 255.0f); Width = 60f; Height = 20.0f }
    { Position = Vector2f(30.0f, 210.0f); Width = 270f; Height = 20.0f }
    { Position = Vector2f(30.0f, 165.0f); Width = 180f; Height = 20.0f }
    { Position = Vector2f(250.0f, 165.0f); Width = 250.0f; Height = 20.0f }
    { Position = Vector2f(380.0f, 30.0f); Width = 20.0f; Height = 225.0f } // вертикальный столб
    { Position = Vector2f(30.0f, 120.0f); Width = 80.0f; Height = 20.0f } 
    { Position = Vector2f(310.0f, 120.0f); Width = 70.0f; Height = 20.0f }
    { Position = Vector2f(190.0f, 75.0f); Width = 120.0f; Height = 20.0f }
    { Position = Vector2f(450.0f, 525.0f); Width = 120.0f; Height = 20.0f }
    { Position = Vector2f(650.0f, 525.0f); Width = 120.0f; Height = 20.0f }
    { Position = Vector2f(550.0f, 480.0f); Width = 250.0f; Height = 20.0f }
    { Position = Vector2f(400.0f, 435.0f); Width = 150.0f; Height = 20.0f }
    { Position = Vector2f(570.0f, 390.0f); Width = 250.0f; Height = 20.0f }
    { Position = Vector2f(400.0f, 345.0f); Width = 100.0f; Height = 20.0f }
    { Position = Vector2f(700.0f, 345.0f); Width = 80.0f; Height = 20.0f }
    { Position = Vector2f(600.0f, 300.0f); Width = 80.0f; Height = 20.0f }
    { Position = Vector2f(650.0f, 255.0f); Width = 120.0f; Height = 20.0f }
    { Position = Vector2f(680.0f, 210.0f); Width = 120.0f; Height = 20.0f }
    { Position = Vector2f(440.0f, 210.0f); Width = 120.0f; Height = 20.0f }
    { Position = Vector2f(540.0f, 165.0f); Width = 100.0f; Height = 20.0f }
    { Position = Vector2f(720.0f, 165.0f); Width = 100.0f; Height = 20.0f }
    { Position = Vector2f(440.0f, 120.0f); Width = 260.0f; Height = 20.0f }
    { Position = Vector2f(400.0f, 75.0f); Width = 100.0f; Height = 20.0f }
    { Position = Vector2f(670.0f, 75.0f); Width = 100.0f; Height = 20.0f }

]

let enemies = [
    { Position = Vector2f(60.0f, 421.0f); Size = enemySize; Direction = 1.0f; Speed = enemySpeed }
    { Position = Vector2f(60.0f, 151.0f); Size = enemySize; Direction = -1.0f; Speed =  enemySpeed }
    { Position = Vector2f(740.0f, 556.0f); Size = enemySize; Direction = -1.0f; Speed =  enemySpeed }
    { Position = Vector2f(500.0f, 196.0f); Size = enemySize; Direction = -1.0f; Speed =  enemySpeed }
]

let initialPosition = Vector2f(50.0f, float32 windowHeight - platformHeight - squareSize)// Начальная позиция квадрата
let initialCoinAnimation = { CurrentFrame = 0; FrameTime = 0.0f }

let initalState = { 
    Position = initialPosition
    VerticalSpeed = 0.0f
    isJumping = false
    Coins = initialCoins
    CollectedCoins= 0
    Enemies= enemies
    CoinAnimation=  initialCoinAnimation
    }

let door = {
    Position = Vector2f(750.0f, 35.0f); Size = Vector2f(20.0f, 40.0f)
}

let window = RenderWindow(VideoMode(uint32 windowWidth, uint32 windowHeight), "Platformer")
window.SetFramerateLimit(60u)
window.Closed.Add(fun _ -> window.Close()) 

let stopwatch = Stopwatch.StartNew()
