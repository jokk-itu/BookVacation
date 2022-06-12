import { publish } from 'gh-pages';

publish(
  'build',
  {
    branch: 'gh-pages',
    repo: 'https://github.com/jokk-itu/BookVacation.git',
    user: {
      name: 'Joachim Køcher Kelsen',
      email: 'jokk@itu.dk'
    },
    dotfiles: true
  },
  () => {
    console.log('Deploy Complete!');
  }
);
