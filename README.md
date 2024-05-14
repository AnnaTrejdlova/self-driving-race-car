# Self-driving race car in Unity ML agents
## Installation
Guide available on: `https://unity-technologies.github.io/ml-agents/Installation/`

My recommended steps for cloning the repository (due to bugs in Release 21):
```
git clone --shallow-since=2023-10-10 -b develop https://github.com/Unity-Technologies/ml-agents.git ml-agents
cd ml-agents
git checkout f3dc8f615044c9226c7e7ed308e0aadc1def3b4d
```

In root ml-agents folder run to observe training:
`tensorboard --logdir results --port 6006`

`cd %USERPROFILE%/Documents/Programming/ml-agents`
`conda activate mlagents`
`mlagents-learn --force`
`mlagents-learn config/sac/Car.yaml --run-id=fixedTeleport`
`mlagents-learn config/Car-sac-gail-bc-sp.yaml --run-id=sac-gail-bc-sp`
`mlagents-learn config/Car-sac-gail-bc-sp.yaml --run-id=sac-gailconcurrent --env="C:\Users\atrej\Unity\ML URP\Build_training_env\RacingGame" --num-envs=4`

```
cd %USERPROFILE%/Documents/Programming/ml-agents
conda activate mlagents
mlagents-learn config/Car-sac-gail-bc-sp.yaml --run-id=sac-gailconcurrent4 --env="C:\Users\atrej\Unity\ML URP\Build_training_env\RacingGame" --num-envs=4 --force

```