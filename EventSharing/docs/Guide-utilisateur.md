# Guide utilisateur EventSharing

## 1. Créer un compte

1. Ouvrir **Register** dans la barre de navigation.
2. Saisir une adresse e-mail et un mot de passe.
3. Se connecter avec **Login**.

La configuration actuelle exige la confirmation du compte avant la connexion si le fournisseur d'e-mail est configuré pour l'application.

## 2. Consulter un événement

Ouvrir **Événements** pour voir les événements à venir. Le filtre **Toutes les catégories** permet de limiter la liste à une catégorie donnée. Sélectionner **Voir le détail** pour consulter la description, les dates, le créateur et le nombre de participants.

## 3. Créer un événement

Après connexion :

1. sélectionner **Créer un événement** ;
2. renseigner le nom, la description, les dates et une catégorie ;
3. valider avec **Créer**.

La date de fin doit être postérieure à la date de début. Le créateur devient automatiquement propriétaire de l'événement.

## 4. Participer

Sur le détail d'un événement, cliquer sur **Participer**. Le bouton devient **Ne plus participer** si l'utilisateur est déjà inscrit.

## 5. Modifier ou supprimer

Seul le créateur voit et peut utiliser les actions **Modifier** et **Supprimer**. Une suppression retire également les participations associées.

## 6. Gérer les catégories

Les utilisateurs connectés peuvent ouvrir **Catégories** pour créer, modifier ou consulter une catégorie. Une catégorie utilisée par un événement ne peut pas être supprimée ; il faut d'abord modifier ou supprimer les événements concernés.

## Dépannage

- Si la page affiche une erreur de base de données, appliquer les migrations avec `dotnet ef database update`.
- Si l'inscription exige une confirmation indisponible, vérifier la configuration d'e-mail ou désactiver cette exigence uniquement pour un environnement local maîtrisé.
- En cas d'accès refusé à une modification, vérifier que le compte connecté est bien le créateur de l'événement.
