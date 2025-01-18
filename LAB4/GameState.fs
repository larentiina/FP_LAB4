module GameState
open SFML.Window
open SFML.Graphics
open SFML.System
open GameConsts


type Coin = {
    Position: Vector2f
    Size: float32
    
}

type Platform = {
    Position: Vector2f
    Width: float32
}

type Enemy = { Position: Vector2f; Size: float32 }

type GameState = {
    Position : Vector2f
    VerticalSpeed : float32
    isJumping: bool
    Coins: Coin list
    CollectedCoins: int
}

type Door = {
    Position: Vector2f
    Size: Vector2f
}

let initialCoins: Coin list = [
    { Position = Vector2f(120.0f, 460.0f); Size = 20.0f }
    { Position = Vector2f(150.0f, 460.0f); Size = 20.0f }
    { Position = Vector2f(180.0f, 460.0f); Size = 20.0f }
    // { Position = Vector2f(500.0f, 270.0f); Size = 20.0f }
    // { Position = Vector2f(650.0f, 220.0f); Size = 20.0f }
]

let finishCoins = List.length initialCoins
let platforms: Platform list = [
    { Position = Vector2f(100.0f, 500.0f); Width = platformWidth }
    { Position = Vector2f(300.0f, 450.0f); Width = platformWidth }  
    { Position = Vector2f(500.0f, 400.0f); Width = platformWidth }  
    { Position =  Vector2f(700.0f, 350.0f); Width = platformWidth } 
    { Position =  Vector2f(0.0f, float32 windowHeight - platformHeight); Width = float32 windowWidth }
]

let enemies: Enemy list = [
    { Position = Vector2f(300.0f, 540.0f); Size = 30.0f }
    { Position = Vector2f(610.0f, 370.0f); Size = 30.0f }
    { Position = Vector2f(700.0f, 300.0f); Size = 30.0f }
]

let initialPosition = Vector2f(375.0f, float32 windowHeight - platformHeight - squareSize)// Начальная позиция квадрата

let initalState = { 
    Position = initialPosition
    VerticalSpeed = 0.0f
    isJumping = false
    Coins = initialCoins
    CollectedCoins= 0
    }

let door = {
    Position = Vector2f(780.0f, 530.0f); Size = Vector2f(20.0f, 40.0f)
}
let window = RenderWindow(VideoMode(uint32 windowWidth, uint32 windowHeight), "Jumping Square")
window.SetFramerateLimit(60u)
window.Closed.Add(fun _ -> window.Close()) // это чтобы при нажатии на крестик игра закрывалась