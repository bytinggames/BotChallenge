using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using JuliHelper;
using NAudio.Wave;
using NVorbis.NAudioSupport;
using Microsoft.Xna.Framework;
using System.IO;

namespace JuliHelper.Audio
{
    public class Sfx3D
    {
        public Sfx3D(SoundEffectInstance sfx, Vector3 pos, float volume, bool global = true, float exponent = 1f)
        {
            this.sfx = sfx;
            this.pos = pos;
            this.global = global;
            this.volume = volume;
        }

        public SoundEffectInstance sfx;
        public Vector3 pos;
        public bool global;
        public float volume;
        public float exponent;
    }

    public static class SoundPlayer
    {
        private static float _musicVolume;
        public static float musicVolume
        {
            get { return _musicVolume; }
            set 
            {
                _musicVolume = Math.Min(1, Math.Max(0, value));
            }
        }

        private static float _soundVolume;
        public static float soundVolume
        {
            get { return _soundVolume; }
            set
            {
                _soundVolume = Math.Min(1, Math.Max(0, value));
            }
        }
        
        private static bool muted;

        public static void Clear()
        {
            for (int i = 0; i < sfxInstances.Count; i++)
            {
                sfxInstances[i].sfx.Stop();
                sfxInstances[i].sfx.Dispose();
            }
            sfxInstances.Clear();

            StopMusic();
        }

        public static void StopSong(string cue)
        {
            Music music = GetSong(cue);
            music.Stop();
            musicInstances.Remove(music);
        }

        public static long GetMusicPosition(string cue)
        {
            Music music = GetSong(cue);
            if (music != null)
                return music.GetPosition();
            else
                return -1;
        }

        //static BlockAlignReductionStream stream;
        //static DirectSoundOut output = null;
        //static WaveChannel32 wv;

        public class Music
        {
            VorbisWaveReader waveReader;
            MyChannel32 channel;
            WaveOut waveOut;
            public string currentSong;

            float myVolume;

            public Music(string cue, string file, float myVolume)
            {
                this.myVolume = myVolume;
                //if (this.myVolume * musicVolume > 0)
                {
                    Stop();


                    waveReader = new NVorbis.NAudioSupport.VorbisWaveReader(file);

                    channel = new MyChannel32(waveReader);
                    channel.Volume = this.myVolume * musicVolume;
                    waveOut = new WaveOut();
                    waveOut.Init(channel);
                   
                    waveOut.Play();
                }

                currentSong = cue;
            }

            public void Stop(bool clearCurrentSongString = true)
            {
                if (waveOut != null)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                }

                if (channel != null)
                    channel.Dispose();

                if (waveReader != null)
                    waveReader.Dispose();

                if (clearCurrentSongString)
                    currentSong = "";
            }

            public void UpdateMusicVolume()
            {
                if (channel != null)
                    channel.Volume = musicVolume;

                if (musicVolume > 0 && muted)
                    PlayMusic(currentSong, true);

                if (musicVolume <= 0 && !muted)
                    Stop(false);

                muted = musicVolume <= 0;
            }

            public void SetMusicVolume(float myVolume)
            {
                this.myVolume = myVolume;
                channel.Volume = this.myVolume * musicVolume;
            }

            internal long GetPosition()
            {
                return channel.Position;
                //return waveOut.GetPosition();
            }
        }

        static AudioEmitter emitter;
        static AudioListener listener;

        static List<Sfx3D> sfxInstances;

        static Matrix cameraTransform;

        static List<Music> musicInstances;

        public static void Initialize()
        {
            musicVolume = 1f;
            soundVolume = 1f;
            
            muted = false;

            sfxInstances = new List<Sfx3D>();
            
            //soundEffect = SoundEffect.FromStream(File.OpenRead(G.exeDir + "\\Content\\Sounds\\flashlight_on.wav"));
            //sfx = new SoundEffect(File.ReadAllBytes(G.exeDir + "\\Content\\Sounds\\flashlight_on.wav"), 44100, AudioChannels.Stereo);
            emitter = new AudioEmitter();
            listener = new AudioListener();

            cameraTransform = Matrix.Identity;

            musicInstances = new List<Music>();

            //PlaySound3D("flashlight_on", new Vector3(10f,0,0));
        }

        public static void SetMusicVolume(float volume)
        {
            musicVolume = volume;

            if (musicInstances != null)
                for (int i = 0; i < musicInstances.Count; i++)
                {
                    musicInstances[i].UpdateMusicVolume();
                }

        }
        public static void SetSoundVolume(float volume)
        {
            soundVolume = volume;

            if (sfxInstances != null)
                for (int i = 0; i < sfxInstances.Count; i++)
                {
                    sfxInstances[i].sfx.Volume = sfxInstances[i].volume * soundVolume;
                }
        }

        public static void PlaySound(string cue, float myVolume = 1, float pitch = 0, float pan = 0f)
        {
            ContentLoader.sounds[cue].Play(Math.Min(1, myVolume * soundVolume), pitch, pan);
        }

        public static void PlayMusic(string cue, bool replayIfPlayed, float volume = 1f)
        {
            bool alreadyPlaying = false;
            if (musicInstances.Count > 0)
            {
                for (int i = 0; i < musicInstances.Count;)
                {
                    if (musicInstances[i].currentSong != cue || replayIfPlayed)
                    {
                        musicInstances[i].Stop();
                        musicInstances.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                        alreadyPlaying = true;
                    }
                }
            }


            
            if (!alreadyPlaying)
            {
                string file = Path.Combine(G.exeDir, "Content", "Music", cue + ".ogg");
                musicInstances.Add(new Music(cue, file, volume));
            }
        }

        public static void AddMusic(string cue, bool replayIfPlayed, float volume = 1)
        {
            if (musicInstances.Count > 0)
            {
                for (int i = 0; i < musicInstances.Count; i++)
                {
                    if (musicInstances[i].currentSong == cue)
                    {
                        if (replayIfPlayed)
                        {
                            musicInstances[i].Stop();
                            musicInstances.RemoveAt(i);
                            break;
                        }
                        else
                            return;
                    }
                }
            }
            
            string file = Path.Combine(G.exeDir, "Content","Music", cue + ".ogg");
            musicInstances.Add(new Music(cue, file, volume));
        }

        /// <summary>
        /// stops the song if muted, starts it again when unmuted
        /// </summary>
        public static void SetSongVolumeStopIfMuted(string cue, bool replay, float volume)
        {
            if (volume > 0)
            {
                if (SoundPlayer.GetSong(cue) == null)
                    SoundPlayer.AddMusic(cue, true, volume);
                else
                    SoundPlayer.SetSongVolume(cue, volume);
            }
            else
            {
                if (SoundPlayer.GetSong(cue) != null)
                    SoundPlayer.StopSong(cue);
            }
        }

        public static void SetSongVolume(string cue, float volume)
        {
            GetSong(cue).SetMusicVolume(volume);
        }

        public static Music GetSong(string cue)
        {
            return musicInstances.Find(f => f.currentSong == cue);
        }

        private static void ClearMusic()
        {
            musicInstances.ForEach(f => f.Stop());
            musicInstances.Clear();
        }

        public static void StopMusic()
        {
            ClearMusic();
        }

        public static void Dispose()
        {
            StopMusic();
        }

        public static void UpdatePositions(Matrix _cameraTransform, GameTime gameTime)
        {
            cameraTransform = _cameraTransform;

            for (int i = 0; i < sfxInstances.Count;i++)
            {
                if (sfxInstances[i].sfx.State == SoundState.Stopped)
                {
                    sfxInstances.RemoveAt(i);
                    i--;
                }
                else if (sfxInstances[i].global)
                {
                    emitter.Position = Vector3.Transform(sfxInstances[i].pos, cameraTransform) * sfxInstances[i].exponent;
                    sfxInstances[i].sfx.Apply3D(listener, emitter);
                }
            }
        }

        public static Sfx3D PlaySound3DGlobal(string cue, Vector3 position, bool loop = false, float myVolume = 1f, float exponent = 1f)
        {
            position = new Vector3(position.X, position.Z, position.Y);
            Sfx3D sfx = new Sfx3D(ContentLoader.sounds[cue].CreateInstance(), position, myVolume);
            sfx.sfx.IsLooped = loop;
            sfx.sfx.Volume = myVolume * soundVolume;
            sfx.exponent = exponent;

            sfxInstances.Add(sfx);
            emitter.Position = Vector3.Transform(position, cameraTransform);
            //float distance = emitter.Position.Length();
            emitter.Position = emitter.Position * sfx.exponent;
            sfx.sfx.Pitch = 1;

            sfx.sfx.Apply3D(listener, emitter);
            sfx.sfx.Play();

            return sfx;
        }
        public static Sfx3D PlaySound3DLocal(string cue, Vector3 position, bool loop = false, float myVolume = 1f, float exponent = 1f)
        {
            position = new Vector3(position.X, position.Z, position.Y);
            Sfx3D sfx = new Sfx3D(ContentLoader.sounds[cue].CreateInstance(), position, myVolume, false);
            sfx.sfx.IsLooped = loop;
            sfx.sfx.Volume = myVolume * soundVolume;
            sfx.exponent = exponent;

            sfxInstances.Add(sfx);
            emitter.Position = position * sfx.exponent;

            sfx.sfx.Apply3D(listener, emitter);
            sfx.sfx.Play();

            return sfx;
        }

        public static void SetPosition(Sfx3D sfx, Vector3 position)
        {
            position = new Vector3(position.X, position.Z, position.Y);
            sfx.pos = position;
            if (sfx.global)
                emitter.Position = Vector3.Transform(position, cameraTransform) * sfx.exponent;
            else
                emitter.Position = position * sfx.exponent;
            sfx.sfx.Apply3D(listener, emitter);
        }
    }
}
