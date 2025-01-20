module GameConsts

open SFML.Graphics
open SFML.System
open SFML.Audio

let windowWidth = 800
let windowHeight = 600

let platformHeight = 30.0f

let squareSize = 20.0f

let gravity = 1.0f
let jumpHeight = -10.0f 
let moveSpeed = 5.0f

let enemySize = 15.0f
let enemySpeed = 2.0f

let coinSize = 15.0f

let font = new Font("Mario-Party-Hudson-Font.ttf")
let coinCounterPosition = Vector2f(35.0f, 20.0f)

let coinSoundBuffer = SoundBuffer("sounds/coin.wav")
let coinSound = Sound(coinSoundBuffer)
let damageSoundBuffer = SoundBuffer("sounds/damage.wav")
let damageSound = Sound(damageSoundBuffer)
