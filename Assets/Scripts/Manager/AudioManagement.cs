using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AudioManagement : MonoBehaviour
{
    [SerializeField] private AudioSource audioMusicSource;
    [SerializeField] private AudioSource audioSoundSource;
    [SerializeField] private AudioSource audioUISource;
    [SerializeField] private AudioSource audioVoiceoverSource;
    [SerializeField] private AudioClip[] audioMusicClip = new AudioClip[2];
    [SerializeField] private AudioClip[] audioSoundClip = new AudioClip[3];
    [SerializeField] private AudioClip[] voiceoverAudioClip = new AudioClip[1];
    [SerializeField] private AudioClip[] uiAudioClip = new AudioClip[1];
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject musicFXIcon;
    [SerializeField] private GameObject soundFXIcon;
    [SerializeField] private GameObject UI_SF_Icon;
    [SerializeField] private GameObject voiceoverFX_Icon;
    [SerializeField] private GameObject musicFXIconOff;
    [SerializeField] private GameObject soundFXIconOff;
    [SerializeField] private GameObject UI_SF_IconOff;
    [SerializeField] private GameObject voiceoverFX_IconOff;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Slider voiceoverSlider;
    [SerializeField] private TextMeshProUGUI musicFX_Text;
    [SerializeField] private TextMeshProUGUI soundFX_Text;
    [SerializeField] private TextMeshProUGUI uiFX_Text;
    [SerializeField] private TextMeshProUGUI voiceoverFX_Text;
    [SerializeField] private Button soundIcon_On;
    [SerializeField] private Button soundIcon_Off;
    [SerializeField] private Button musicIcon_On;
    [SerializeField] private Button musicIcon_Off;
    [SerializeField] private Button UIIcon_On;
    [SerializeField] private Button UIIcon_Off;
    [SerializeField] private Button VoiceoverIcon_On;
    [SerializeField] private Button VoiceoverIcon_Off;

    [Header("Get Audio Channels")]
    [SerializeField] string musicVolume = "MusicSFX";
    [SerializeField] string soundVolume = "SoundFX";
    [SerializeField] string uiSFXVolume = "UI_SFX";
    [SerializeField] string voiceoverVolume = "VoiceoverSFX";

    //Random Variables
    private string getCurrentScene = "";
    private float multiplier = 50f;
    [SerializeField] private float _musicVolume = 0.0001f;
    [SerializeField] private float _soundVolume = 0.0001f;
    [SerializeField] private float _UIVolume = 0.0001f;
    [SerializeField] private float _voiceoverVolume = 0.0001f;

    //Holders
    [SerializeField] private float holderMusic = 0.0001f;
    [SerializeField] private float holderSound = 0.0001f;
    [SerializeField] private float holderUI = 0.0001f;
    [SerializeField] private float holderVoiceover = 0.0001f;

    void Awake()
    {
        getCurrentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(PlayMusicAudio());
    }

    private void OnEnable()
    {
        //Slider Listeners
        musicSlider.onValueChanged.AddListener(HandleSliderMusic);
        soundSlider.onValueChanged.AddListener(HandleSliderSound);
        uiSlider.onValueChanged.AddListener(HandleSliderUI);
        voiceoverSlider.onValueChanged.AddListener(HandleSliderVoiceover);

        //Icon Listeners
        musicIcon_On.onClick.AddListener(SetMusicVolumeToZero);
        musicIcon_Off.onClick.AddListener(SetMusicVolume);
        soundIcon_On.onClick.AddListener(SetSoundVolumeToZero);
        soundIcon_Off.onClick.AddListener(SetSoundVolume);
        UIIcon_On.onClick.AddListener(SetUIVolumeToZero);
        UIIcon_Off.onClick.AddListener(SetUIVolume);
        VoiceoverIcon_On.onClick.AddListener(SetVoiceoverVolumeToZero);
        VoiceoverIcon_Off.onClick.AddListener(SetVoiceoverVolume);
        LoadAudioSettings();
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("musicVolume", _musicVolume);
        PlayerPrefs.SetFloat("soundVolume", _UIVolume);
        PlayerPrefs.SetFloat("UIVolume", _UIVolume);
        PlayerPrefs.SetFloat("voiceoverVolume", _voiceoverVolume);
        PlayerPrefs.Save();
    }

    private void LoadAudioSettings()
    {
        //Update Sliders
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        soundSlider.value = PlayerPrefs.GetFloat("soundVolume");
        uiSlider.value = PlayerPrefs.GetFloat("UIVolume");
        voiceoverSlider.value = PlayerPrefs.GetFloat("voiceoverVolume");

        //Set Text
        musicFX_Text.text = Math.Truncate(musicSlider.value * 100).ToString() + "%";
        soundFX_Text.text = Math.Truncate(soundSlider.value * 100).ToString() + "%";
        uiFX_Text.text = Math.Truncate(uiSlider.value * 100).ToString() + "%";
        voiceoverFX_Text.text = Math.Truncate(voiceoverSlider.value * 100).ToString() + "%";
    }

    #region Button Mute
    private void SetMusicVolumeToZero()
    {
        holderMusic = _musicVolume;
        if (musicSlider.value > musicSlider.minValue)
            musicSlider.value = musicSlider.minValue;
    }

    private void SetMusicVolume()
    {
        if (musicSlider.value == musicSlider.minValue)
            musicSlider.value = holderMusic;
    }

    private void SetSoundVolumeToZero()
    {
        holderSound = _soundVolume;
        if (soundSlider.value > soundSlider.minValue)
            soundSlider.value = soundSlider.minValue;
    }

    private void SetSoundVolume()
    {
        if (soundSlider.value == soundSlider.minValue)
            soundSlider.value = holderSound;
    }

    private void SetUIVolumeToZero()
    {
        holderUI = _UIVolume;
        if (uiSlider.value > uiSlider.minValue)
            uiSlider.value = uiSlider.minValue;
    }

    private void SetUIVolume()
    {
        if (uiSlider.value == uiSlider.minValue)
            uiSlider.value = holderUI;
    }

    private void SetVoiceoverVolumeToZero()
    {
        holderVoiceover = _voiceoverVolume;
        if (voiceoverSlider.value > voiceoverSlider.minValue)
            voiceoverSlider.value = voiceoverSlider.minValue;
    }

    private void SetVoiceoverVolume()
    {
        if (voiceoverSlider.value == voiceoverSlider.minValue)
            voiceoverSlider.value = holderVoiceover;
    }
    #endregion

    private void HandleSliderMusic(float value)
    {
        _musicVolume = value;
        float musicFloat = Mathf.Log10(value) * multiplier;
        musicFX_Text.text = Math.Truncate(musicSlider.value * 100).ToString() + "%";

        if(musicSlider.value == musicSlider.minValue)
        {
            musicFXIcon.SetActive(false);
            musicFXIconOff.SetActive(true);
        } else if(musicSlider.value > musicSlider.minValue)
        {
            musicFXIcon.SetActive(true);
            musicFXIconOff.SetActive(false);
        }
        audioMixer.SetFloat(musicVolume, musicFloat);
    }

    private void HandleSliderSound(float value)
    {
        _soundVolume = soundSlider.value;

        float soundFloat = Mathf.Log10(value) * multiplier;
        soundFX_Text.text = Math.Truncate(soundSlider.value * 100).ToString() + "%";

        if (soundSlider.value == soundSlider.minValue)
        {
            soundFXIcon.SetActive(false);
            soundFXIconOff.SetActive(true);
        }
        else if (soundSlider.value > soundSlider.minValue)
        {
            soundFXIcon.SetActive(true);
            soundFXIconOff.SetActive(false);
        }
        audioMixer.SetFloat(soundVolume, soundFloat);
    }

    private void HandleSliderUI(float value)
    {
        _UIVolume = uiSlider.value;
        float uiSFFloat = Mathf.Log10(value) * multiplier;
        uiFX_Text.text = Math.Truncate(uiSlider.value * 100).ToString() + "%";

        if (uiSlider.value == uiSlider.minValue)
        {
            UI_SF_Icon.SetActive(false);
            UI_SF_IconOff.SetActive(true);
        }
        else if (uiSlider.value > uiSlider.minValue)
        {
            UI_SF_Icon.SetActive(true);
            UI_SF_IconOff.SetActive(false);
        }
        audioMixer.SetFloat(uiSFXVolume, uiSFFloat);
    }

    private void HandleSliderVoiceover(float value)
    {
        _voiceoverVolume = voiceoverSlider.value;

        float voiceoverFloat = Mathf.Log10(value) * multiplier;
        voiceoverFX_Text.text = Math.Truncate(voiceoverSlider.value * 100).ToString() + "%";

        if (voiceoverSlider.value == voiceoverSlider.minValue)
        {
            voiceoverFX_Icon.SetActive(false);
            voiceoverFX_IconOff.SetActive(true);
        }
        else if (voiceoverSlider.value > voiceoverSlider.minValue)
        {
            voiceoverFX_Icon.SetActive(true);
            voiceoverFX_IconOff.SetActive(false);
        }
        audioMixer.SetFloat(voiceoverVolume, voiceoverFloat);
    }

    public void PlaySoundClip(int index)
    {
        audioSoundSource.PlayOneShot(audioSoundClip[index]);
        return;
    }

    public void PlayUIClip(int index)
    {
        audioUISource.PlayOneShot(uiAudioClip[index]);
        return;
    }

    public void PlayVoiceoverAudio(int index)
    {
        audioVoiceoverSource.PlayOneShot(voiceoverAudioClip[index]);
        return;
    }

    private IEnumerator PlayMusicAudio()
    {
        if(getCurrentScene == "ShopMenu")
        {
            audioSoundSource.PlayOneShot(audioSoundClip[0]);
        }

        if (getCurrentScene == "Login_Update_Menu")
        {
            audioMusicSource.Stop();
            audioMusicSource.clip = audioMusicClip[0];
            audioMusicSource.Play();
        }

        if(getCurrentScene == "MainMenu")
        {
            audioMusicSource.Stop();
            audioMusicSource.clip = audioMusicClip[1];
            audioMusicSource.Play();
        }
        yield return new WaitForSeconds(audioMusicSource.clip.length);
    }
}