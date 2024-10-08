﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JetSystems
{
    public class RoadManager : MonoBehaviour
    {
        [Header(" Debug ")]
        public bool DEBUG;
        public int levelToPlay;
        int level;

        [Header(" Road Chunks ")]
        public RoadChunk initialChunk;
        public RoadChunk finishChunk;
        private RoadChunk previousChunk;
        Vector3 finishPos;
        Vector3 spawnPos;

        [Header(" Predefined Levels ")]
        public LevelSequence[] levelSequences;

        List<RoadChunk> levelChunks = new List<RoadChunk>();

        static RoadManager instance;

        private void Awake()
        {
            level = PlayerPrefs.GetInt("LEVEL");
        }

        // Start is called before the first frame update
        void Start()
        {
            instance = this;

            UIManager.onLevelCompleteSet += IncreaseLevelIndex;
            UIManager.onNextLevelButtonPressed += SpawnLevel;
            UIManager.onRetryButtonPressed += RetryLevel;

            SpawnLevel();
        }

        private void OnDestroy()
        {
            UIManager.onLevelCompleteSet -= IncreaseLevelIndex;
            UIManager.onNextLevelButtonPressed -= SpawnLevel;
            UIManager.onRetryButtonPressed -= RetryLevel;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void IncreaseLevelIndex(int useless)
        {
            level++;
            PlayerPrefs.SetInt("LEVEL", level);
        }

        public void SpawnLevel()
        {
            // Delete the children
            ClearLevel();

            levelChunks.Clear();

            spawnPos = Vector3.zero;

            int currentLevel = level;

            if (DEBUG)
                currentLevel = levelToPlay;

            if (currentLevel >= levelSequences.Length)
            {
                SpawnLevelSequence(Random.Range(0, levelSequences.Length));
            }
            else
                SpawnLevelSequence(currentLevel);
        }

        private void SpawnLevelSequence(int currentLevel)
        {
            for (int i = 0; i < levelSequences[currentLevel].chunks.Length; i++)
            {
                RoadChunk chunkToSpawn = levelSequences[currentLevel].chunks[i];
                Instantiate(chunkToSpawn, spawnPos, Quaternion.identity, transform);

                spawnPos.z += chunkToSpawn.length;
                previousChunk = chunkToSpawn;
                levelChunks.Add(chunkToSpawn);
            }

            // We can then spawn the finish chunk
            Instantiate(finishChunk, spawnPos, Quaternion.identity, transform);

            levelChunks.Add(finishChunk);

            // Store the finish pos for progression use
            finishPos = spawnPos;
        }


        private void ClearLevel()
        {
            while (transform.childCount > 0)
            {
                Transform t = transform.GetChild(0);
                t.SetParent(null);
                Destroy(t.gameObject);
            }
        }

        public Vector3 GetFinishLinePosition()
        {
            return finishPos;
        }

        public void RetryLevel()
        {
            ClearLevel();
            spawnPos = Vector3.zero;

            for (int i = 0; i < levelChunks.Count; i++)
            {
                RoadChunk spawnedChunk = Instantiate(levelChunks[i], spawnPos, Quaternion.identity, transform);
                spawnPos.z += levelChunks[i].length;
            }
        }

        public float GetFinishLineZ()
        {
            return finishPos.z;
        }

        public static Vector3 GetFinishPosition()
        {
            return instance.GetFinishLinePosition();
        }
    }

    [System.Serializable]
    public struct LevelSequence
    {
        public RoadChunk[] chunks;
    }
}