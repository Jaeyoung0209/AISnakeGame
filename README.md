# AISnakeGame
A classic snake game with an AI agent that gradually improves performance through CNN and genetic algorithm  
All agents move randomly at the beginning, but they are rewarded for eating the apple and are punished for dying. If they live for too long without eating, they are automatically destroyed.  
Agents with the most reward are more likely to reproduce, carrying on their weights and biases to the next generation.  

The entire project was coded in unity using c#, without any external libraries or plugins.

![TrainingProcess](https://github.com/Jaeyoung0209/AISnakeGame/assets/112497692/bb62af41-ec43-4f91-8ccd-258ddc3caa28)

The above video is what the training process looks like when it runs in Unity.  

![SuccessfulAgent](https://github.com/Jaeyoung0209/AISnakeGame/assets/112497692/5c3c768d-926e-4632-b859-567a0bd43a45)

This is one of the most successful agents that I was able to get to. Theoretically, running the training process longer would yield even better results.
