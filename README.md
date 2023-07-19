<!--
*** Readme created using https://github.com/othneildrew/Best-README-Template 
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]




<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

ZseTimetable is web api to fetch my school's timetable and convert it into standarized json format.

api is available for public use under the address https://zsetimetable.live/api

See also frontend implementation: https://github.com/Mrczarny/zseTimetableFront


## API Reference

#### Get timetable

```
  GET /api/timetable/${type}/${name}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `type` | `string` | **Required**. Type of timetable (class, classroom or teacher) |
| `name` | `string` | **Required**. name of timetable |

#### Get names of all timetables of given type

```
  GET /api/timetable/${type}/allnames
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `type` | `string` | **Required**. Type of timetable (class, classroom or teacher) |

#### Get changes for given day

```
  GET /api/changes/${date}
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `date` | `datetime` | **Required**. Date of the day you want to get changes for |

#### Get all changes for current week

```
  GET /api/changes/week
```

#### Get all changes for today

```
  GET /api/changes/today
```





### Built With

* .NET Core version 3.1 

<!-- GETTING STARTED -->
## Getting Started

### Prerequisites

ZseTimetable uses .NET core 3.1 , so make sure to have it or higher version of it.

<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/Mrczarny/OptiConfig/issues) for a list of proposed features (and known issues).



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.



<!-- CONTACT -->
## Contact

Jan Adamski - strzelec2004@gmail.com

Project Link: [https://github.com/Mrczarny/ZseTimetable](https://github.com/Mrczarny/ZseTimeTable)



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [GitHub Emoji Cheat Sheet](https://www.webpagefx.com/tools/emoji-cheat-sheet)
* [Img Shields](https://shields.io)
* [Choose an Open Source License](https://choosealicense.com)
* [GitHub Pages](https://pages.github.com)
* [Animate.css](https://daneden.github.io/animate.css)
* [Loaders.css](https://connoratherton.com/loaders)
* [Slick Carousel](https://kenwheeler.github.io/slick)
* [Sticky Kit](http://leafo.net/sticky-kit)
* [JVectorMap](http://jvectormap.com)
* [Font Awesome](https://fontawesome.com)





<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/Mrczarny/OptiConfig?style=for-the-badge
[contributors-url]: https://github.com/Mrczarny/OptiConfig/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Mrczarny/OptiConfig?style=for-the-badge
[forks-url]: https://github.com/Mrczarny/OptiConfig/network/members
[stars-shield]: https://img.shields.io/github/stars/Mrczarny/OptiConfig?style=for-the-badge
[stars-url]: https://github.com/Mrczarny/OptiConfig/stargazers
[issues-shield]: https://img.shields.io/github/issues/Mrczarny/OptiConfig?style=for-the-badge
[issues-url]: https://github.com/Mrczarny/OptiConfig/issues
[license-shield]: https://img.shields.io/github/license/Mrczarny/OptiConfig?style=for-the-badge
[license-url]: https://github.com/Mrczarny/OptiConfig/blob/master/LICENSE.txt
