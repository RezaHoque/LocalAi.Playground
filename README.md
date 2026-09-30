# LocalAi.Playground
A .NET console app for experimenting with locally hosted LLMs using Ollama.

## Hardware

Local LLM performance depends heavily on the model size and your hardware.

Smaller quantized models such as 7B/8B models can run on many modern computers. A supported NVIDIA GPU can significantly improve generation speed.

The project itself does not require a specific GPU.

## Requirements

You'll need:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Ollama](https://ollama.com/)
- At least one Ollama model

A dedicated GPU is recommended for good performance but is not strictly required.

## 1. Install Ollama

Download and install Ollama:

https://ollama.com/download

Verify the installation:

```bash
ollama --version
```

## 2. Download a model

For example:

```bash
ollama pull qwen3:8b
```

You can also use another model supported by Ollama.

For example:

```bash
ollama pull llama3.1:8b
```

See your installed models with:

```bash
ollama list
```

## 3. Clone the repository

```bash
git clone https://github.com/RezaHoque/LocalAi.Playground.git
cd LocalAi.Playground
```

## 4. Run

```bash
dotnet run
```

The application queries Ollama and displays the models installed on your machine:

```text
Local AI

Available models:

1. qwen3:8b (5.2 GB)
2. llama3.1:8b (4.9 GB)

Select model:
```

Choose a model:

```text
Select model: 1
```

Then start chatting:

```text
You: What is dependency injection?

AI: Dependency injection is a design pattern where an object's
dependencies are provided externally rather than created internally.
```

Responses are streamed as they're generated.
