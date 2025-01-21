module CoinBehaviour

open SFML.Window
open SFML.Graphics
open SFML.System
open SFML.Audio
open GameConsts
open GameState

let drawCoins (window: RenderWindow) (coins: Coin list) =
    coins |> List.iter (fun coin ->
        let coinShape = RectangleShape(Size = Vector2f(coinSize,  coinSize), FillColor = Color.Yellow)
        coinShape.Position <- coin.Position
        window.Draw(coinShape)
    )

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