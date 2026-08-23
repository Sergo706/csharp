export default defineNuxtConfig({
  modules: [
    '@nuxt/content',
    '@nuxt/hints',
    '@nuxt/image',
    '@nuxt/ui',
    '@nuxtjs/i18n',
    '@nuxtjs/seo',
    'nuxt-feedme',
    '@nuxt/fonts',
    '@vueuse/nuxt',
    'nuxt-llms'
  ],
  nitro: {
    prerender: {
      autoSubfolderIndex: false,
      crawlLinks: true,
      routes: ['/', '/sitemap.xml', '/feed.xml', '/feed.atom', '/feed.json']
    },
  },
  routeRules: {
    '/': { prerender: true, cache: { maxAge: 60 * 60 * 24 * 30 } },
    '/account': { prerender: false, ssr: false },
    '/api/**': {
      prerender: false,
      proxy: {
        to: 'http://localhost:5113/api/**',
        fetchOptions: {
          credentials: 'include',
        },
        cookieDomainRewrite: {
          '*': '',
        },
        cookiePathRewrite: {
          '*': '/',
        },
      },
    }
  },
  typescript: {
    tsConfig: {
      compilerOptions: {
        strictNullChecks: true,
        strict: true
      }
    }
  },
  icon: {
    customCollections: [
      {
        prefix: 'custom',
        dir: './app/assets/icons',
      },
    ],
    clientBundle: {
      scan: true,
      includeCustomCollections: true,
    },
    provider: 'iconify',
  },
  css: [
    './app/assets/css/index.css'
  ],
  site: {
    url: 'https://convertor.riavzon.com',
    name: 'Document Convertor',
    description: 'Convert PDF files CSV, MD files and more to any formats',
    indexable: true,
    defaultLocale: 'en',
  },
  app: {
    head: {
      title: 'Document Convertor',
      htmlAttrs: {
        lang: 'en',
      },
      link: [
        { rel: 'icon', type: 'image/svg+xml', href: '/favicon.svg' },
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
        { rel: 'icon', type: 'image/png', sizes: '96x96', href: '/favicon-96x96.png' },
        { rel: 'apple-touch-icon', type: 'image/png', sizes: '180x180', href: '/apple-touch-icon.png' },
        { rel: 'manifest', href: '/site.webmanifest' },
        { rel: 'alternate', type: 'application/rss+xml', title: 'Document Convertor Blog RSS', href: '/feed.xml' },
        { rel: 'alternate', type: 'application/atom+xml', title: 'Document Convertor Blog Atom', href: '/feed.atom' },
      ],
      meta: [
        { name: 'theme-color', content: '#FFFDF7', media: '(prefers-color-scheme: light)' },
        { name: 'theme-color', content: '#0A0908', media: '(prefers-color-scheme: dark)' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1.0' },
        { name: 'charset', content: 'utf-8' },
        { name: 'color-scheme', content: 'light dark' },
      ]
    }
  },
  linkChecker: {
    failOnError: false,
    excludeLinks: ['/account']
  },
  sitemap: {
    zeroRuntime: true,
    defaults: {
      lastmod: new Date().toISOString(),
    }
  },
  ogImage: {
    zeroRuntime: true,
    componentDirs: ['app/components/OgImage']
  },
  feedme: {
    defaults: {
      common: false,
    },
    feeds: {
      common: {
        feed: {
          title: 'Doc Convertor Blog',
          description: 'Latest articles and updates from the us',
          id: 'https://convertor.riavzon.com/',
          link: 'https://convertor.riavzon.com/',
          language: 'en',
          favicon: 'https://convertor.riavzon.com/favicon.ico',
          copyright: `© ${new Date().getFullYear()} Doc Convertor`,
          author: {
            name: 'Riavzon',
            link: 'https://convertor.riavzon.com',
          },
        },
        revisit: '6h',
        fixDateFields: true,
        templateMapping: ['', 'meta', 'meta.feedme'],
        mapping: [['link', 'path']],
        charset: 'utf-8',
        collections: ['blog'],
      },
      routes: {
        '/feed.atom': { type: 'atom1' },
        '/feed.json': { type: 'json1' },
        '/feed.xml': { type: 'rss2' },
      }
    },
  },
  devtools: { enabled: true },
  compatibilityDate: '2024-04-03',
  content: {
    build: {
      markdown: {
        highlight: {
          theme: {
            default: 'light-plus',
            light: 'light-plus',
            dark: 'dracula'
          },
        },
      }
    }
  }
})