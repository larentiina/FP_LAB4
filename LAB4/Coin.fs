module CoinBehaviour

open SFML.Window
open SFML.Graphics
open SFML.System
open SFML.Audio
open GameConsts
open GameState

let updateCoinAnimation (coinAnimationState: CoinAnimationState) deltaTime : CoinAnimationState =
    let newFrameTime = coinAnimationState.FrameTime + deltaTime
    if newFrameTime >= animationSpeed then
        { CurrentFrame = (coinAnimationState.CurrentFrame + 1) % totalFrames
          FrameTime = 0.2f }
    else
        { coinAnimationState with FrameTime = newFrameTime }

let drawCoins (window: RenderWindow) (coins: Coin list) (coinAnimation: CoinAnimationState) (deltaTime: float32) =
    coins |> List.iter (fun coin ->
        let sprite = Sprite(coinTexture)

        let updatedCoinAnimation = updateCoinAnimation coinAnimation deltaTime

        let frameX = updatedCoinAnimation.CurrentFrame * frameWidth
        sprite.TextureRect <- IntRect(frameX, 0, frameWidth, frameHeight)
        sprite.Position <- coin.Position

        window.Draw(sprite)
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
