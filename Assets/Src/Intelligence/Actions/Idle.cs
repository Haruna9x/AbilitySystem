﻿using UnityEngine;

namespace Intelligence {

    public class Idle : CharacterAction {

        private float timer;
        public float idleTime;

        public override void OnStart() {
            NavMeshAgent agent = entity.GetComponent<NavMeshAgent>();
            agent.ResetPath();
        }

        public override bool OnUpdate() {
            timer += Time.deltaTime;
            return timer > idleTime;
        }

    }
}