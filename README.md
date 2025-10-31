# 🎮 Joy Controller - Contrôle de souris par manette Xbox

Contrôlez votre curseur Windows avec une manette Xbox ou compatible XInput grâce à une **interface graphique moderne et intuitive**.

## 🚀 Installation

### Prérequis
- Windows 10/11
- .NET 6.0 ou supérieur
- Une manette Xbox (ou compatible XInput)

### Étapes d'installation

1. **Restaurer les dépendances NuGet** :
```bash
dotnet restore
```

2. **Compiler le projet** :
```bash
dotnet build
```

3. **Lancer l'application** :
```bash
dotnet run
```

## 🎯 Interface Graphique

L'application dispose d'une **interface graphique complète** qui affiche :
- 📊 **Visualiseurs de joysticks en temps réel** - Voyez exactement comment vos joysticks réagissent
- 🎯 **Indicateurs de boutons** - État visuel de tous les boutons
- 🔌 **État de connexion** - Sachez instantanément si votre manette est connectée
- ⚙️ **Accès rapide à la configuration** - Un clic pour ajuster vos paramètres
- 🔔 **Icône dans la barre des tâches** - Minimisez l'application sans la fermer

## 🎯 Contrôles

| Contrôle | Action |
|----------|--------|
| **Joystick gauche** | Déplacer le curseur avec vélocité variable et accélération |
| **Joystick droit (vertical)** | Scroll avec vélocité variable |
| **Bouton A** | Clic gauche |
| **Bouton B** | Clic droit |
| **Bouton X** | Clic milieu |

## ⚙️ Configuration

### Interface graphique de configuration

L'application dispose d'une **fenêtre de configuration intuitive** accessible directement depuis l'interface principale en cliquant sur le bouton **"⚙️ Configuration"**

La fenêtre de configuration vous permet d'ajuster :
- 🎯 **Sensibilité du curseur** (5-50)
- 🎮 **Zone morte (deadzone)** (3000-15000) - élimine le drift du joystick
- 🚀 **Courbe d'accélération** (1.0-3.0) - contrôle la progressivité du mouvement
- ⚡ **Multiplicateur de vitesse** (0.5-3.0) - vitesse globale du curseur
- 📜 **Zone morte du scroll** (3000-12000)
- 🔄 **Sensibilité du scroll** (10-100)

### Calibrage en temps réel

La fenêtre inclut un **outil de calibrage** qui affiche en direct :
- Position du joystick gauche (axe X et Y)
- Position du joystick droit (scroll)
- Détection automatique des zones mortes

Cliquez sur **"▶️ Démarrer calibrage"** pour tester vos réglages en temps réel !

### Sauvegarde automatique

Vos paramètres sont **automatiquement sauvegardés** dans :
```
%APPDATA%\JoyController\config.json
```

### Valeurs recommandées :
- **Sensibilité** : entre 10 et 20 (usage général), 25-40 (gaming)
- **Deadzone** : entre 5000 et 10000 (8000 par défaut)
- **Courbe d'accélération** : entre 1.0 (linéaire) et 2.0 (forte accélération)
- **Multiplicateur** : entre 1.0 et 2.5
- **Scroll deadzone** : entre 6000 et 8000
- **Scroll sensibilité** : entre 20 et 50

## 📋 Fonctionnalités

- ✅ **Interface graphique moderne et intuitive**
- ✅ **Visualisation en temps réel** des joysticks et boutons
- ✅ **Icône dans la barre des tâches** avec menu contextuel
- ✅ **Interface de configuration** avec calibrage en temps réel
- ✅ Déplacement fluide du curseur avec vélocité variable
- ✅ Accélération progressive (petits mouvements précis, grands mouvements rapides)
- ✅ Zone morte configurable pour éviter le drift
- ✅ Clics gauche/droit/milieu
- ✅ Scroll avec vélocité variable via joystick droit
- ✅ Détection automatique de la manette
- ✅ Gestion de la déconnexion de la manette
- ✅ **Sauvegarde persistante** des paramètres
- ✅ Contrôle Start/Stop depuis l'interface

## 🛠️ Technologies utilisées

- **Windows Forms** : Interface graphique moderne
- **SharpDX.XInput** : Bibliothèque pour accéder aux entrées XInput
- **Win32 API** : Pour contrôler le curseur et les événements de souris
- **.NET 6.0** : Framework d'exécution

## 📝 Notes

- L'application doit être exécutée avec les permissions suffisantes pour contrôler la souris
- Seule la première manette connectée (UserIndex.One) est utilisée
- Le programme tourne à environ 100 FPS pour une réactivité optimale

## 🐛 Dépannage

**Manette non détectée** :
- Vérifiez que la manette est bien connectée
- Testez la manette dans les paramètres Windows (Paramètres > Périphériques > Contrôleurs de jeu)
- Assurez-vous que les pilotes Xbox sont installés

**Curseur qui bouge tout seul (drift)** :
- Ouvrez la fenêtre de configuration (bouton Back ou SHIFT au démarrage)
- Augmentez la valeur de **"Zone morte (deadzone)"**
- Utilisez le calibrage en temps réel pour trouver la bonne valeur

**Curseur trop lent/rapide** :
- Ajustez **"Sensibilité du curseur"** et **"Multiplicateur vitesse"** dans la configuration
- Testez en temps réel avec l'outil de calibrage

**Accélération trop forte/faible** :
- Modifiez **"Courbe d'accélération"** dans la configuration :
  - `1.0` = mouvement linéaire (pas d'accélération)
  - `1.5` = accélération modérée (par défaut)
  - `2.0-3.0` = forte accélération

**Scroll trop lent/rapide** :
- Ajustez **"Sensibilité scroll"** et **"Zone morte scroll"** dans la configuration
- Augmentez la sensibilité pour un scroll plus rapide
- Diminuez pour plus de précision

**L'application ne démarre pas** :
- Vérifiez que .NET 6.0 ou supérieur est installé
- Essayez de reconstruire le projet avec `dotnet build`

## 📄 Licence

Projet libre d'utilisation et de modification.
