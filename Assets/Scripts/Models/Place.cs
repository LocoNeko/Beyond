using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public enum Hemisphere : int { North, South }

    [System.Serializable]
    public class Place
    {
        [SerializeField]public string Name { get; protected set; }
        [SerializeField] public Hemisphere Hemisphere { get; protected set; }
        [SerializeField] public Gametime Gametime { get; protected set; }
        [SerializeField] public int Length { get; protected set; }
        [SerializeField] public int Width { get; protected set; }
        [SerializeField] public int Height { get; protected set; }
        [SerializeField] public int LowestY { get; protected set; }
        [SerializeField] public List<BeyondGroup> BeyondGroups { get; protected set; }

        public Place(string s = "A test place", int l = 1000, int w = 1000, int h = 200, int ly = -20)
        {
            Name = s;
            Hemisphere = Hemisphere.North;
            Gametime = new Gametime();
            Length = l;
            Width = w;
            Height = h;
            LowestY = ly;
            BeyondGroups = new List<BeyondGroup>();
            Debug.Log(string.Format("New place '{0}' created" , Name));
        }

        public void update(float deltatime)
        {
            Gametime.Update(deltatime);
        }

        public Season GetSeason()
        {
            return Gametime.GetSeason(Hemisphere);
        }

    }
}