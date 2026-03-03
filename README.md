# VERA Sandbox Experience

The **VERA Sandbox Experience** is a mock VR experiment designed to demonstrate how to integrate the **VERA (Virtual Experience Research Accelerator)** toolkit into an existing Unity project. It serves as a step-by-step reference for researchers who want to convert a standard Unity VR experience into a **VERA-enabled remote experiment** capable of deployment, participant management, and automated data collection.

This repository contains two branches that illustrate the full integration process:

* **`main`** – The base Unity VR experience without VERA integration.
* **`VERA`** – The completed version of the same experience with VERA fully integrated.

The goal of this sandbox is to show how to go from **`main` → `VERA`**, demonstrating the steps required to enable remote deployment, experiment configuration, survey delivery, and data logging using the VERA platform.

> ⚠️ **Note:** This project is a *demonstration experiment*. It mimics the structure of a real study but is not intended to produce meaningful research results.

---

# Overview

The sandbox simulates a simple VR task in which participants shoot pumpkins with a laser. The experience varies conditions across two independent variables and records several performance metrics.

Researchers can:

* Explore the base Unity experiment
* Configure a matching experiment on the VERA platform
* Integrate the VERA SDK
* Build and deploy the experience to the web
* Collect simulated participant data remotely

---

# Experimental Structure

This demo experiment uses the following components.

## Independent Variables

Two experimental factors are used:

**Environment**

* `Ice`
* `Desert`

**AimType**

* `GoodAim`
* `BadAim`

These variables should be defined in the **Experimental Design** section when creating your experiment on the VERA portal.

---

## Experiment Measures

Two types of data are recorded in this demo.

### Shot-Level Data

Each laser shot is logged individually.

CSV columns:

* `hitOrMiss` *(bool)*

Optional:

* `experimentBlock` *(int)*
* `experimentRound` *(int)*

### Round Summary Data

Each round records the participant's overall accuracy.

CSV columns:

* `roundAccuracy` *(float)*

Optional:

* `experimentBlock` *(int)*
* `experimentRound` *(int)*

---

## Survey

Participants complete a brief survey asking them about their **confidence in their shooting ability**.

Example questions may include:

* "How confident were you in your ability to hit the pumpkins?"
* Likert scale confidence rating.

Only one survey is required for this demo.

---

# Running the Sandbox

The sandbox can be explored in two ways:

1. **Explore the base experiment**
   Open the `main` branch, run the Unity project, and interact with the mock experiment.

2. **Integrate and deploy with VERA**
   Follow the steps below to configure the experiment on the VERA platform and connect it to the Unity project.

---

# Step-by-Step Setup

## 1. Explore the Base Experience

Open the **`main` branch** of the repository.

You can run the experience locally in Unity to explore the VR task. At this stage:

* Surveys are not displayed
* Data is not logged to VERA
* The experiment is not connected to the VERA backend

This represents a typical starting point for a Unity VR experiment.

---

# 2. Create a VERA Account

Visit the VERA platform:

**https://vera-xr.io**

Create an account or log in.

---

# 3. Create a New Experiment

Create a new experiment in the VERA dashboard and configure the following sections.

## Experimental Design

Define two independent variables:

| Variable    | Values          |
| ----------- | --------------- |
| Environment | Ice, Desert     |
| AimType     | GoodAim, BadAim |

---

## Experiment Measures

Create two CSV data outputs.

### Shot Data

Columns:

* `hitOrMiss` *(bool)*

Optional:

* `experimentBlock` *(int)*
* `experimentRound` *(int)*

### Round Accuracy

Columns:

* `roundAccuracy` *(float)*

Optional:

* `experimentBlock` *(int)*
* `experimentRound` *(int)*

---

## Surveys

Create a survey asking participants about their **confidence in their shooting performance**.

You may include one or more questions using a rating scale.

---

## Distribution

For this demo, choose:

**Manual Remote Distribution**

Any distribution method will work, but this is the simplest option for the sandbox.

---

## Other Settings

All other experiment settings may remain **default or blank**.

---

# 4. Install the VERA Unity Package

Install the VERA SDK through the **Unity Package Manager** using the Git URL:

```
https://github.com/ucf-research/vera.git
```

---

# 5. Authenticate Unity with VERA

Open the VERA settings panel in Unity:

```
Menu Bar → VERA → Settings
```

Click:

**Authenticate**

This links your Unity editor session to your VERA account and experiment.

---

# 6. Configure the Experiment Code

Open:

```
ExperimentManager.cs
```

Inside the script you will see several sections labeled:

```
VERA SANDBOX NOTE
```

Follow the instructions in those sections to connect the Unity experience to the experiment you created on the VERA platform.

These steps demonstrate how to:

* Register experiment variables
* Record measures
* Trigger surveys
* Send data to the VERA backend

---

# 7. Build and Upload the Experiment

Return to:

```
Menu Bar → VERA → Settings
```

Click:

**Build and Upload Experiment**

VERA will automatically:

1. Build the project for **WebXR**
2. Package the experiment
3. Upload it to the VERA platform

---

# 8. Run the Experiment

Once uploaded:

1. Go to your experiment in the **VERA dashboard**
2. Click the **link icon**
3. Copy the generated participant link

Open the link in a browser to run the experience exactly as a participant would.

---

# 9. Start Data Collection

When you are ready to begin collecting real data:

1. Return to the VERA dashboard
2. Click **Go Live**
3. Distribute the participant link to your study participants

VERA will automatically handle:

* Participant sessions
* Data logging
* Survey responses
* Experiment condition assignment

---

# What This Sandbox Demonstrates

This repository shows the full workflow for turning a Unity VR experience into a **remotely deployable experiment** using VERA:

* Configuring an experiment in the VERA portal
* Integrating the VERA Unity SDK
* Logging experiment data
* Deploying WebXR builds
* Running remote participant studies

By comparing the **`main`** and **`VERA`** branches, researchers can clearly see the changes required to add VERA support to an existing project.

---
