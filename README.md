# Self-driving race car in Unity ML agents
## Installation
Guide available on: `https://unity-technologies.github.io/ml-agents/Installation/`

My recommended steps for cloning the repository (due to bugs in Release 21):
```
git clone --shallow-since=2023-10-10 -b develop https://github.com/Unity-Technologies/ml-agents.git ml-agents
cd ml-agents
git checkout f3dc8f615044c9226c7e7ed308e0aadc1def3b4d
```
`--shallow-since` allows for faster download, but doesn't include history

## Running the training
```
cd %USERPROFILE%/Documents/Programming/ml-agents
conda activate mlagents
mlagents-learn config/Car-ppo-cont.yaml --run-id=ppo_gail_cont --env="%USERPROFILE%/Unity/ML URP/Build_training_env/RacingGame" --num-envs=4
```

In root ml-agents folder run to observe training:
`tensorboard --logdir results --port 6006`