/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        unity: {
          dark: "#1E1E1E",
          darker: "#191919",
          blue: "#3F8FD2",
          green: "#7BC74D",
          gray: "#B4B4B4",
          lightGray: "#3C3C3C",
        },
      },
      fontFamily: {
        inter: ["Inter", "sans-serif"],
        mono: ["JetBrains Mono", "monospace"],
      },
    },
  },
  plugins: [],
};
