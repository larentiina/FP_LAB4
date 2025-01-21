module CoinBehaviour

open SFML.Window
open SFML.Graphics
open SFML.System
open SFML.Audio
open GameConsts
open GameState

let coinTexture = new Texture("sprite/coin.png")
let frameWidth = 16
let frameHeight = 16
let totalFrames = 15

let mutable currentFrame = 1
let mutable frameTime = 0.2f
let animationSpeed = 0.3f 

let drawCoins (window: RenderWindow) (coins: Coin list) (deltaTime: float32) =
    coins |> List.iter (fun coin ->
        let sprite = Sprite(coinTexture)
        
        frameTime <- frameTime + deltaTime
        if frameTime >= animationSpeed then
            currentFrame <- (currentFrame + 1) % totalFrames
            frameTime <- 0.0f
        
        let frameX = currentFrame * frameWidth
        sprite.TextureRect <- IntRect(frameX, 0, frameWidth, frameHeight)
        sprite.Position <- coin.Position

        window.Draw(sprite)
    )

// let drawCoins (window: RenderWindow) (coins: Coin list) =
//     coins |> List.iter (fun coin ->
//         let coinShape = RectangleShape(Size = Vector2f(coinSize,  coinSize), FillColor = Color.Yellow)
//         coinShape.Position <- coin.Position
//         window.Draw(coinShape)
//     )

let coinCounter (coins: int) =
    let label = Text(sprintf "Coins Left: %d" coins, font)
    label.Position <- coinCounterPosition
    label.CharacterSize <- 30u  
    label.FillColor <- Color.Red
    label

let checkCoinCollision (position: Vector2f) (squareSize: float32) (coins: Coin list) =
    let newCoins, collectedCoins =
        coins |> List.fold (fun (remaining, collected) coin ->
            let coinBounds = FloatRect(coin.Position.X, coin.Position.Y,  coinSize,  coinSize)
            
            let squareBounds = FloatRect(position.X , position.Y, squareSize, squareSize)
            
            if coinBounds.Intersects(squareBounds) then
                coinSound.Play()
                (remaining, coin::collected)  
            else
                (coin::remaining, collected)  
        ) ([], [])

    newCoins, List.length collectedCoins  