using System;
using System.Collections.Generic;
using Godot;

namespace Utils
{
	public class SoundUtils
	{
		#region Sound Helpers
        public static AudioStreamPlayer3D GetRandomSoundFromList(List<AudioStreamPlayer3D> sounds)
        {
            var random = new Random();
            var index = random.Next(0, sounds.Count - 1);

            return sounds[index];
        }
        #endregion
	}
}