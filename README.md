# 🎮 Joy Controller - Contrôle de souris par manette Xbox StickControl

Contrôlez votre curseur Windows avec une manette Xbox ou compatible XInput.

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

## 🎯 Contrôles

| Contrôle | Action |
|----------|--------|
| **Joystick gauche** | Déplacer le curseur avec vélocité variable et accélération |
| **Joystick droit (vertical)** | Scroll avec vélocité variable |
| **Bouton A** | Clic gauche |
| **Bouton B** | Clic droit |
| **Bouton X** | Clic milieu |
| **Bouton Start** | Quitter l'application |

## ⚙️ Configuration

Vous pouvez ajuster la sensibilité et l'accélération dans le fichier `Program.cs` :

```csharp
const int sensitivity = 15; // Augmentez pour un curseur plus rapide
const short deadzone = 8000; // Zone morte du joystick

// Ligne 104: Courbe d'accélération (1.0 = linéaire, 1.5 = accélération modérée, 2.0 = forte accélération)
Math.Pow(Math.Abs(velocityX), 1.5)

// Ligne 109: Multiplicateur de vitesse (augmentez pour curseur plus rapide)
int deltaX = (int)(accelX * sensitivity * 2);
```

### Valeurs recommandées :
- **sensitivity** : entre 10 et 30
- **deadzone** : entre 5000 et 10000
- **courbe d'accélération** : entre 1.0 (linéaire) et 2.0 (forte accélération)
- **multiplicateur** : entre 1 et 3

## 📋 Fonctionnalités

- ✅ Déplacement fluide du curseur avec vélocité variable
- ✅ Accélération progressive (petits mouvements précis, grands mouvements rapides)
- ✅ Zone morte configurable pour éviter le drift
- ✅ Clics gauche/droit/milieu
- ✅ Scroll avec vélocité variable via joystick droit
- ✅ Détection automatique de la manette
- ✅ Gestion de la déconnexion de la manette

## 🛠️ Technologies utilisées

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

**Curseur qui bouge tout seul** :
- Augmentez la valeur de `deadzone` dans le code

**Curseur trop lent/rapide** :
- Ajustez la valeur de `sensitivity`
- Modifiez le multiplicateur (ligne 109 : `sensitivity * 2`)

**Accélération trop forte/faible** :
- Changez l'exposant de la courbe d'accélération (ligne 104)
- `1.0` = mouvement linéaire (pas d'accélération)
- `1.5` = accélération modérée (par défaut)
- `2.0` = forte accélération

**Scroll trop lent/rapide** :
- Modifiez le multiplicateur de vélocité (ligne 169 : `velocity * 30`) dans `Program.cs`
- Augmentez ou diminuez la zone morte du scroll (ligne 161 : `scrollDeadzone = 6000`)

## 📄 Licence

Projet libre d'utilisation et de modification.
