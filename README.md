# Self-driving race car in Unity ML agents
## Installation
Guide available on: `https://unity-technologies.github.io/ml-agents/Installation/`

My recommended steps for cloning the repository (due to bugs in Release 21):
```
git clone --shallow-since=2023-10-10 -b develop https://github.com/Unity-Technologies/ml-agents.git ml-agents
cd ml-agents
git checkout f3dc8f615044c9226c7e7ed308e0aadc1def3b4d
```

`cd %USERPROFILE%/Documents/Programming/ml-agents`
`conda activate mlagents`
`mlagents-learn --force`
`mlagents-learn config/sac/Car.yaml --run-id=fixedTeleport`

```
cd %USERPROFILE%/Documents/Programming/ml-agents
conda activate mlagents

```