# 🎃 PumpkinJourney

<div align="center">

**An Educational 2D Platformer Game built with Unity**

[![Unity](https://img.shields.io/badge/Unity-2022.3+-blue.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)](https://www.microsoft.com/windows)

*A fun educational platformer where you help a pumpkin character explore, learn, and battle slimes!*

[🎮 Play Demo](#) • [📖 Documentation](#) • [🐛 Report Bug](https://github.com/yourusername/PumpkinJourney/issues) • [✨ Request Feature](https://github.com/yourusername/PumpkinJourney/issues)

![PumpkinJourney Screenshot](https://via.placeholder.com/800x400/FF6B35/FFFFFF?text=PumpkinJourney+Screenshot)

</div>

---

## 📋 Table of Contents

- [🎮 About](#-about)
- [✨ Features](#-features)
- [🎯 Gameplay](#-gameplay)
- [🚀 Installation](#-installation)
- [🎵 Audio Setup](#-audio-setup)
- [🛠️ Technical Details](#-technical-details)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)
- [🙏 Acknowledgments](#-acknowledgments)

## 🎮 About

**PumpkinJourney** is an educational 2D platformer game built with Unity. Control a cute pumpkin character as you explore levels, collect keys, answer educational quizzes, and battle slimes in this fun learning adventure!

### 🎯 Game Objectives
- 🏃‍♂️ Explore platforming worlds
- 🔑 Collect secret keys
- 📚 Answer educational quiz questions
- 🐸 Defeat slime enemies
- 🏆 Complete levels to unlock new challenges

## ✨ Features

### 🎮 Core Gameplay
- **Smooth Controls**: Walk, run, jump, and slide mechanics with Unity physics
- **Combat System**: Strategic jump attacks and collision-based damage
- **Health Management**: 3-heart system with game over mechanics
- **4 Unique Levels**: Progressive difficulty with unlock system

### 🎓 Educational Elements
- **Interactive Quizzes**: Educational questions on science, math, and geography
- **Multiple Choice**: A/B/C/D answer format with instant feedback
- **Level-Specific Content**: Different quiz topics per level

### 🎵 Immersive Audio
- **Dynamic BGM**: Scene-adaptive background music
- **25+ SFX**: Complete sound effects for all interactions
- **Professional Audio**: Volume controls and sound management

### 🎨 Polish & UI
- **Intuitive Menus**: Main menu, pause, game over, and level select screens
- **Visual Feedback**: Smooth animations and particle effects
- **HUD Elements**: Health, coins, and progress indicators

## 🎯 Gameplay

### 🕹️ Controls
- **A/D** or **Arrow Keys**: Move left/right
- **Space**: Jump
- **S** or **Down Arrow**: Slide
- **E**: Interact (near objects)
- **ESC** or **P**: Pause game

### 🎮 How to Play
1. **Launch Game** from main menu
2. **Explore Levels** - find keys while avoiding slimes
3. **Find Key Prompt** - press E to start quiz
4. **Answer Questions** - correct answers unlock keys
5. **Reach Door** - use key to open level exit
6. **Complete Levels** - unlock next challenges

### 💡 Pro Tips
- **Jump Attacks**: Jump on slimes from above for instant defeat
- **Slide Mechanics**: Use slides to dodge enemies or access tight spaces
- **Quiz Timing**: Answer quickly for better scores
- **Coin Collection**: Gather coins for high scores

## 🛠️ Technical Details

### 🏗️ Architecture
- **Singleton Pattern**: GameManager, AudioManager, UIManager
- **Component-based**: Modular Unity components
- **Event-driven**: UI and game state management
- **ScriptableObjects**: AudioData, QuizData for easy content editing

### 🎮 Core Systems
- **Player Controller**: Physics-based movement with comprehensive input handling
- **Enemy AI**: Patrol and chase behaviors with collision detection
- **Quiz Engine**: Multiple choice questions with validation and feedback
- **Audio Manager**: Scene-adaptive music and comprehensive SFX library

### 📊 Game Balance
- **Progressive Difficulty**: 4 levels with increasing challenge
- **Health System**: 3 hearts with damage feedback
- **Quiz Timing**: Strategic time limits for engagement
- **Combat**: Jump attacks and collision mechanics

### 🎚️ Audio Features
- **Scene-based switching**: Musik ganti otomatis per scene
- **BGM continuity**: Tidak restart saat level reset
- **SFX interrupt**: Pause sound dapat di-stop
- **Volume control**: Atur volume via code atau Inspector

## 🚀 Installation

### 📋 Prerequisites
- **Unity 2022.3.47f1** or later
- **Windows** (primary), cross-platform support
- **TextMeshPro** (auto-installed by Unity)

### 🛠️ Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/PumpkinJourney.git
   cd PumpkinJourney
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Add project from PumpkinJourney folder
   - Open with Unity 2022.3+

3. **Setup Audio Assets** (see Audio Setup section)

4. **Configure Build Settings**
   - File → Build Settings
   - Add scenes: `homePage`, `gameplay`, `gameplay2`, etc.
   - Set `homePage` as Scene 0

5. **Hit Play!** 🎮
   - Test from homePage scene
   - Verify all features work correctly

## 🎵 Audio Setup

### 📥 Download Audio Assets

**Background Music:**
- [MainBGM.wav](https://freesound.org/people/LittleRobotSoundFactory/packs/16881/) - Gameplay music
- [HomePageBGM.wav](https://freesound.org/people/Headphaze/packs/13768/) - Menu music
- [QuizBGM.wav](https://freesound.org/people/Headphaze/packs/13768/) - Quiz music

**Sound Effects Pack:**
- [Complete 8-bit SFX Pack](https://freesound.org/people/LittleRobotSoundFactory/packs/16881/) - All game sounds

### 🔧 Setup Instructions

1. **Download** all audio files from the links above
2. **Place** in `Assets/Audio/` folder
3. **Rename** files to match AudioType enum names
4. **Assign** to `Assets/Resources/Audio/GameAudioData.asset`
5. **Test** audio in Unity Editor

### 📋 Audio Files Required

**BGM (3 files):**
- MainBGM.wav, HomePageBGM.wav, QuizBGM.wav

**SFX (25+ files):**
- Jump.wav, Death.wav, Slide.wav, Land.wav, Footstep.wav
- ButtonClick.wav, UiPopup.wav, DoorOpen.wav, DoorLocked.wav
- EnemyHit.wav, EnemyDeath.wav, PlayerHurt.wav
- CorrectAnswer.wav, WrongAnswer.wav, QuizStart.wav, QuizComplete.wav
- LevelComplete.wav, Score.wav, KeyCollect.wav, CandyCollect.wav

## 🤝 Contributing

We welcome contributions! Please follow these guidelines:

### 🚀 Development Process
1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/AmazingFeature`)
3. **Commit** your changes (`git commit -m 'Add AmazingFeature'`)
4. **Push** to the branch (`git push origin feature/AmazingFeature`)
5. **Open** a Pull Request

### 📋 Code Standards
- **Naming**: PascalCase for classes, camelCase for variables
- **Documentation**: XML comments for public methods
- **Architecture**: Clear separation of concerns
- **Testing**: Test in Unity Editor before committing

### 🎯 Feature Ideas
- **New Levels**: Add scenes with unique quiz content
- **Enemy Variety**: Different AI patterns and abilities
- **Power-ups**: Item effects and visual feedback
- **UI Polish**: Better animations and transitions

### 🐛 Bug Reports
Found a bug? [Open an issue](https://github.com/yourusername/PumpkinJourney/issues) with:
- Unity version
- Steps to reproduce
- Expected vs actual behavior
- Screenshots/logs if applicable

## 📄 License

**PumpkinJourney** is open source software licensed under the MIT License.

Copyright © 2024 - Free for educational and personal use

## 🙏 Acknowledgments

- **Unity Technologies** - Game engine
- **FreeSound.org** - Audio assets
- **TextMeshPro** - UI text rendering
- **Universal RP** - Rendering pipeline

---

<div align="center">

**Made with ❤️ for educational gaming**

[⭐ Star this repo](https://github.com/yourusername/PumpkinJourney) • [🐛 Report Issues](https://github.com/yourusername/PumpkinJourney/issues) • [💡 Request Features](https://github.com/yourusername/PumpkinJourney/issues)

</div>

---

<div align="center">

**Made with ❤️ using Unity**

[🎮 Play Demo](#) • [📖 Full Documentation](#) • [🐛 Report Issues](https://github.com/yourusername/PumpkinJourney/issues) • [✨ Request Features](https://github.com/yourusername/PumpkinJourney/issues)

**Enjoy PumpkinJourney! 🎃**

</div>
