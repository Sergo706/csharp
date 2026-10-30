export default defineNuxtConfig({
  modules: [
    '@nuxt/content',
    '@nuxt/hints',
    '@nuxt/image',
    '@nuxt/ui',
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
    '/dashboard/**': { prerender: false, ssr: false },
    '/confirm-email/**': { prerender: false },
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
    '~/assets/css/index.css'
  ],
  site: {
    url: 'https://convertor.riavzon.com',
    name: 'Document Convertor',
    description: 'Convert PDF files CSV, MD files and more to any formats',
    indexable: true,
    defaultLocale: 'en',
  },
  llms: {
    domain: 'https://convertor.riavzon.com',
    title: 'Document Convertor',
    description: 'A browser-based document conversion platform supporting 22 explicitly allowlisted conversions across PDF, Markdown, office, spreadsheet, image, and HTML formats.',
    sections: [
      {
        title: 'Platform Overview',
        description: 'Convert documents directly in your browser and download the result immediately. Conversion is available without an account; signed-in users can also keep a history containing the output filename, source format, target format, and conversion timestamps.'
      },
      {
        title: 'Complete Conversion Matrix',
        description: `The server supports the following 22 directed conversion paths. A checkmark means that the input in that row can be converted to the output in that column.

| Input / output | PDF | Markdown (MD) | DOCX | JPG | PPTX | HTML |
| --- | :---: | :---: | :---: | :---: | :---: | :---: |
| PDF | — | ✓ | ✓ | ✓ | ✓ | ✓ |
| Markdown (MD) | ✓ | — | ✓ | — | — | ✓ |
| DOCX | ✓ | ✓ | — | — | — | — |
| CSV | ✓ | ✓ | — | — | — | — |
| XLS | ✓ | ✓ | — | — | — | — |
| XLSX | ✓ | ✓ | — | — | — | — |
| ODS | ✓ | — | — | — | — | — |
| JPG | ✓ | — | — | — | — | — |
| PPTX | ✓ | ✓ | — | — | — | — |
| HTML | ✓ | ✓ | — | — | — | — |`
      },
      {
        title: 'Upload, Convert & Download',
        description: 'Upload your file through the web interface, select your target format, and convert. Files may be up to 10 MB. Once the conversion is complete, the file is returned as a download with a sanitized, timestamped filename.',
        links: [
          {
            title: 'Document Convertor',
            description: 'Open the browser-based document conversion experience.',
            href: 'https://convertor.riavzon.com/'
          }
        ]
      },
      {
        title: 'Format-Aware Conversion Engines',
        description: 'Document Convertor routes each supported pair through a purpose-built pipeline. LibreOffice handles office, spreadsheet, image, PDF, and Markdown conversions; MarkItDown produces Markdown from PDF, DOCX, CSV, XLS, XLSX, PPTX, and HTML; and pdf2htmlEX produces self-contained HTML from PDF with embedded styling, fonts, images, JavaScript, and outlines. PDF-to-Markdown first creates an intermediate DOCX before Markdown extraction.'
      },
      {
        title: 'Accounts & Conversion History',
        description: 'Accounts are optional for conversion. The platform supports local registration with confirmed email, sign-in through Google or GitHub, profile and account management, password recovery, refresh tokens, and two-factor authentication. When a signed-in user converts a document, the platform records conversion metadata that is available from the authenticated /api/history endpoint. for authenticated users'
      },
      {
        title: 'Validation, Isolation & Service Limits',
        description: 'The server rejects empty files, unsupported extensions, unsupported source/target pairs, and MIME families that conflict with a recognized filename extension. Every job runs in a unique temporary work directory, conversion processes use timeouts, and temporary files are removed after the request. Limits are 5 conversion requests per minute, 10 authentication requests per minute, and 50 total requests per minute, partitioned by authenticated user or client IP.'
      },
      {
        title: 'Privacy and File storage',
        description: 'The Server doesnt store any files the user provides, it does the work, and delete it completely when done, to insure maximum privacy for user sensitive documents, the only thing that stored are the action and metadata about a file for authenticated users. Accounts arent required.'
      },
      {
        title: 'Service Definition',
        description: 'This is not a paid service and completely free to use in the allowed limits, and rather act as a utility website'
      },
      {
        title: 'Content',
        description: 'Pages and guides published through the Document Convertor Nuxt Content collection. Each listed page has a raw Markdown representation for LLMs and is included in the consolidated /llms-full.txt export.',
        contentCollection: 'content'
      },
      {
        title: 'Blog',
        description: 'Document conversion articles, platform updates, tutorials, and feeds.',
        links: [
          {
            title: 'Document Convertor Blog',
            description: 'Read articles, tutorials, and platform updates.',
            href: 'https://convertor.riavzon.com/blog'
          },
          {
            title: 'RSS Feed',
            description: 'Follow the Document Convertor blog in RSS format.',
            href: 'https://convertor.riavzon.com/feed.xml'
          },
          {
            title: 'Atom Feed',
            description: 'Follow the Document Convertor blog in Atom format.',
            href: 'https://convertor.riavzon.com/feed.atom'
          },
          {
            title: 'JSON Feed',
            description: 'Consume the Document Convertor blog in JSON Feed format.',
            href: 'https://convertor.riavzon.com/feed.json'
          },
          {
            title: 'Author’s Blog',
            description: 'Read announcements, technical articles, and project updates from the author.',
            href: 'https://riavzon.com/writing'
          }
        ]
      },
      {
        title: 'More From the Author',
        description: 'Document Convertor is part of the Riavzon ecosystem. Explore the author’s main site and other applications.',
        links: [
          {
            title: 'Riavzon',
            description: 'The author’s main hub and the home of the wider Riavzon ecosystem.',
            href: 'https://riavzon.com'
          },
          {
            title: 'Solana Bots',
            description: 'A desktop application and CLI for managing multiple wallets, executing mass token swaps, and running volume bots while keeping keys and data on the user’s device.',
            href: 'https://solana.riavzon.com/'
          },
          {
            title: 'IP Shield',
            description: 'An IP intelligence and threat-detection API and dashboard with geolocation, ISP and ASN data, VPN, proxy, and Tor signals, threat levels, bot detection, API-key management, and usage analytics.',
            href: 'https://ip-shield.riavzon.com/'
          }
        ]
      },
      {
        title: 'Fast Navigation for LLMs',
        description: 'For a specific page, use its raw Markdown link in the Content section instead of downloading the complete export. Use /llms-full.txt only when consolidated context from every content page is required.',
        links: [
          {
            title: 'Home Page Raw Markdown',
            description: 'Raw Markdown for the Document Convertor home page. Human-readable page: https://convertor.riavzon.com/',
            href: 'https://convertor.riavzon.com/raw/index.md'
          },
          {
            title: 'About Page Raw Markdown',
            description: 'Raw Markdown for the Document Convertor about page. Human-readable page: https://convertor.riavzon.com/about',
            href: 'https://convertor.riavzon.com/raw/about.md'
          }
        ]
      }
    ],
    notes: [
      'The conversion matrix above is the server-enforced allowlist: formats cannot be combined arbitrarily.',
      'The maximum upload size is 10 MB, and conversion requests are limited to 5 per minute per authenticated user or client IP.',
      'The platform accepts anonymous conversions; authentication is required only for account-specific features such as conversion history.',
      'For announcements, tutorials, and technical articles, visit https://convertor.riavzon.com/blog or the author’s writing at https://riavzon.com/writing.'
    ],
    full: {
      title: 'Document Convertor Full Content',
      description: 'Consolidated machine-readable content from every page in the Document Convertor content collection.'
    }
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