# 🎵 Panduan Setup Audio untuk PumpkinJourney

## 📋 Daftar Audio yang Dibutuhkan

### 🎼 Background Music (Loop)
- [ ] `mainBGM.wav/mp3` - Musik latar utama game
- [ ] `quizBGM.wav/mp3` - Musik saat mengerjakan kuis
- [ ] `windAmbient.wav/mp3` - Suara angin ambient (loop)

### 👤 Player Sounds
- [ ] `jump.wav` - Suara karakter loncat
- [ ] `death.wav` - Suara karakter mati
- [ ] `slide.wav` - Suara karakter slide
- [ ] `land.wav` - Suara landing setelah loncat
- [ ] `footstep.wav` - Suara langkah kaki

### 🎮 UI Sounds
- [ ] `buttonClick.wav` - Suara klik tombol
- [ ] `buttonHover.wav` - Suara hover tombol
- [ ] `uiPopup.wav` - Suara UI muncul
- [ ] `uiClose.wav` - Suara UI hilang

### 🏰 Gameplay Sounds
- [ ] `keyCollect.wav` - Suara ambil kunci
- [ ] `doorLocked.wav` - Suara pintu terkunci
- [ ] `doorOpen.wav` - Suara pintu terbuka
- [ ] `levelComplete.wav` - Suara di pintu level selanjutnya
- [ ] `score.wav` - Suara dapat poin
- [ ] `candyCollect.wav` - Suara ambil candy
- [ ] `powerUp.wav` - Suara power up

### ❓ Quiz Sounds
- [ ] `correctAnswer.wav` - Suara jawaban benar
- [ ] `wrongAnswer.wav` - Suara jawaban salah
- [ ] `quizComplete.wav` - Suara berhasil menyelesaikan kuis
- [ ] `quizStart.wav` - Suara mulai kuis
- [ ] `timeWarning.wav` - Suara peringatan waktu

## 🔧 Setup di Unity

### 1. Buat Audio Mixer
1. Buka Window > Audio > Audio Mixer
2. Buat 3 grup: Master, BGM, SFX, Ambient
3. Assign ke AudioManager

### 2. Import Audio Files
1. Letakkan semua file audio di `Assets/Sound/`
2. Set Compression Format: ADPCM untuk SFX, Vorbis untuk BGM
3. Load Type: Decompress On Load untuk SFX penting

### 3. Setup AudioManager
1. Buat GameObject "AudioManager" di scene
2. Attach script AudioManager.cs
3. Assign semua AudioClip di Inspector
4. Assign AudioMixer

### 4. Integrasi dengan Script Lain

#### Pumpkin.cs - Player Sounds
```csharp
// Di method Jump()
AudioManager.Instance.PlayJumpSound();

// Di method Die()
AudioManager.Instance.PlayDeathSound();

// Di method Slide()
AudioManager.Instance.PlaySlideSound();
```

#### KeyItem.cs - Key Sounds
```csharp
// Di method Collect()
AudioManager.Instance.PlayKeyCollectSound();
```

#### DoorExit.cs - Door Sounds
```csharp
// Saat pintu terkunci
AudioManager.Instance.PlayDoorLockedSound();

// Saat pintu terbuka
AudioManager.Instance.PlayDoorOpenSound();

// Saat level complete
AudioManager.Instance.PlayLevelCompleteSound();
```

#### QuizManager.cs - Quiz Sounds
```csharp
// Saat mulai kuis
AudioManager.Instance.PlayQuizStartSound();
AudioManager.Instance.PlayQuizBGM();

// Saat jawaban benar
AudioManager.Instance.PlayCorrectAnswerSound();

// Saat jawaban salah
AudioManager.Instance.PlayWrongAnswerSound();

// Saat kuis selesai
AudioManager.Instance.PlayQuizCompleteSound();
AudioManager.Instance.PlayMainBGM(); // Kembali ke BGM utama
```

#### UIManager.cs - UI Sounds
```csharp
// Di method ShowPressEIcon()
AudioManager.Instance.PlayUIPopupSound();

// Di method HidePressEIcon()
AudioManager.Instance.PlayUICloseSound();
```

### 5. Volume Controls (Optional)
Tambahkan UI slider untuk kontrol volume:
- Master Volume
- SFX Volume
- BGM Volume

## 📝 Tips Audio Design

### 🎵 BGM Guidelines
- Main BGM: Energetic tapi tidak mengganggu gameplay
- Quiz BGM: Lebih fokus, membantu konsentrasi
- Volume: 30-50% dari SFX

### 🔊 SFX Guidelines
- Durasi: 0.5-2 detik untuk efek
- Format: WAV untuk kualitas, MP3 untuk ukuran
- Volume: Balance dengan BGM

### 🎚️ Audio Mixer Setup
```
Master (0 dB)
├── BGM (-10 dB)
├── SFX (0 dB)
│   ├── Player (-5 dB)
│   ├── UI (-5 dB)
│   ├── Gameplay (-5 dB)
│   └── Quiz (-5 dB)
└── Ambient (-20 dB)
```

## ✅ Testing Checklist

- [ ] Semua audio terdengar jelas
- [ ] Volume balance antara BGM dan SFX
- [ ] Tidak ada clipping atau distortion
- [ ] Audio stops/pauses saat game pause
- [ ] BGM berubah saat quiz mode
- [ ] SFX responsif terhadap input player

## 🎯 File Audio yang Perlu Dibuat/Download

1. **BGM**: Cari royalty-free game music di:
   - OpenGameArt.org
   - FreeMusicArchive.org
   - Incompetech.com

2. **SFX**: Gunakan sumber:
   - Freesound.org
   - Zapsplat.com
   - Buat sendiri dengan Audacity

3. **Voice**: Rekam voice acting untuk:
   - "Ambil kunci dulu!"
   - "Jawaban benar!"
   - "Coba lagi!"

---

**Status**: ⏳ Menunggu file audio untuk di-assign ke AudioManager
