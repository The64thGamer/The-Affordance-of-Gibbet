using Godot;
using System;

public partial class SoundManager : Node
{
	Godot.Collections.Array<AudioStreamPlayer> soundPlayers = new Godot.Collections.Array<AudioStreamPlayer>();
	AudioStreamPlayer musicPlayer = new AudioStreamPlayer();
	const int soundChannels = 4;
	const int simultaniousSoundCache = 3;
	float oldSoundVolume = -443;
	float oldMusicVolume = -443;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		musicPlayer.Bus = "Music";

		for (int i = 0; i < soundChannels * simultaniousSoundCache; i++)
		{
			AudioStreamPlayer player = new AudioStreamPlayer();
			player.Bus = "Sounds";
			soundPlayers.Add(player);
			AddChild(player);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float newVol = PlayerPrefs.GetFloat("SoundVolume");
		if(oldSoundVolume != newVol)
		{
			oldSoundVolume = newVol;
			float vol = Mathf.Lerp(-20,0,newVol);
			if(newVol == 0)
			{
				vol = -1000;
			}
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Sounds"),vol);
		}

		float newMus = PlayerPrefs.GetFloat("MusicVolume");
		if(oldMusicVolume != newMus)
		{
			oldMusicVolume = newMus;
			float vol = Mathf.Lerp(-20,0,newMus);
			if(newMus == 0)
			{
				vol = -1000;
			}
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"),vol);
		}
	}

	public void PlaySound(string name)
	{
		for (int e = 0; e < simultaniousSoundCache; e++)
		{
			//if anyone changes sound channel count this needs to be dynamic
			if(!soundPlayers[e*soundChannels].Playing &&
			!soundPlayers[(e*soundChannels)+1].Playing &&
			!soundPlayers[(e*soundChannels)+2].Playing && 
			!soundPlayers[(e*soundChannels)+3].Playing
			)
			{
				for (int i = 0; i < soundChannels; i++)
				{
					string path = "res://Sounds/" + name + ".ch" + (i+1) + ".wav";
					if(ResourceLoader.Exists(path))
					{
						soundPlayers[(e*soundChannels)+i].Stream = GD.Load<AudioStream>(path);
						soundPlayers[(e*soundChannels)+i].Play();
					}	
				}
				return;
			}		
		}
		
	}
}
