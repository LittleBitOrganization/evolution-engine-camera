# Модуль камеры

## Импорт
``` json
"com.littlebitgames.cameramodule": "https://github.com/LittleBitOrganization/evolution-engine-camera.git"
```

## Зависимости и требования:

1. #### Импортируйте модуль Touch Input
``` json
"com.littlebitgames.touchinput": "https://github.com/LittleBitOrganization/evolution-engine-touch-input.git"
```
2. #### Установите актуальную версию [Zenject](https://github.com/modesttree/Zenject/releases) с помощью <b>.unitypackage</b>

3. #### Установите актуальную версию <b>Cinemachine</b> через встроенный <b>PackageManager</b>

3. #### Установите актуальную версию <b>DoTween</b> через <b>Asset Store</b> или с [офф. сайта](http://dotween.demigiant.com/download.php)

## Инстанцирование и настройка

- В папке Resources создайте конфиг для камеры

![Screenshot_1](https://user-images.githubusercontent.com/23084919/176433705-4bd5f67d-a983-44a5-a620-62a6f37edd0f.png)
 
- Поместите CameraPrefab из папки Camera Module/Runtime/Resources в ProjectContext

- Добавьте ссылку на созданный конфиг в префаб камеры

- Поместите Installer который находится на CameraPrefab в ProjectContext после бинда TouchInputService

### Вы молодец!
