using System;
using simpleDI.Injection;
using UnityEngine;

namespace Aion.Highlights.Data
{
   [CreateAssetMenu(fileName = "TeamKit", menuName = "Data/TeamKit")]
   [Serializable]
   public class TeamKit : ScriptableObject, IInjectable
   {
      // Skin and Hair Color depend on the player, they are not team-wide
      public Color ShirtColor;
      public Color ShortsColor;
      public Color SocksColor;
      public Color ShoesColor = Color.black;
      public Color GlovesColor = Color.yellow;
   }
}